using System;

using Microsoft.ServiceBus;

using NuClear.Jobs;
using NuClear.Messaging.API.Flows;
using NuClear.Replication.OperationsProcessing.Telemetry;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus.Factories;
using NuClear.Security.API.Auth;
using NuClear.Security.API.Context;
using NuClear.Telemetry;
using NuClear.Tracing.API;
using NuClear.ValidationRules.OperationsProcessing.AggregatesFlow;
using NuClear.ValidationRules.OperationsProcessing.AmsFactsFlow;
using NuClear.ValidationRules.OperationsProcessing.FactsFlow;
using NuClear.ValidationRules.OperationsProcessing.MessagesFlow;

using Quartz;

using ValidationRules.Hosting.Common;

namespace NuClear.ValidationRules.Replication.Host.Jobs
{
    [DisallowConcurrentExecution]
    public sealed class ReportingJob : TaskServiceJobBase
    {
        private readonly ITelemetryPublisher _telemetry;
        private readonly IServiceBusSettingsFactory _serviceBusSettingsFactory;
        private readonly KafkaMessageFlowInfoProvider _kafkaMessageFlowInfoProvider;

        private readonly ITracer _tracer;

        public ReportingJob(
            ITelemetryPublisher telemetry,
            IServiceBusSettingsFactory serviceBusSettingsFactory,
            KafkaMessageFlowInfoProvider kafkaMessageFlowInfoProvider,
            IUserContextManager userContextManager,
            IUserAuthenticationService userAuthenticationService,
            IUserAuthorizationService userAuthorizationService,
            ITracer tracer)
            : base(userContextManager, userAuthenticationService, userAuthorizationService, tracer)
        {
            _tracer = tracer;
            _kafkaMessageFlowInfoProvider = kafkaMessageFlowInfoProvider;
            _telemetry = telemetry;
            _serviceBusSettingsFactory = serviceBusSettingsFactory;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            WithinErrorLogging(ReportMemoryUsage);
            WithinErrorLogging(ReportServiceBusQueueLength<FactsFlow, PrimaryProcessingQueueLengthIdentity>);
            WithinErrorLogging(ReportServiceBusQueueLength<AggregatesFlow, FinalProcessingAggregateQueueLengthIdentity>);
            WithinErrorLogging(ReportServiceBusQueueLength<MessagesFlow, MessagesQueueLengthIdentity>);
            WithinErrorLogging(ReportKafkaOffset<AmsFactsFlow, AmsFactsQueueLengthIdentity>);
        }

        private void WithinErrorLogging(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Eception in ReportingJob");
            }
        }

        private void ReportMemoryUsage()
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();
            _telemetry.Publish<ProcessPrivateMemorySizeIdentity>(process.PrivateMemorySize64);
            _telemetry.Publish<ProcessWorkingSetIdentity>(process.WorkingSet64);
        }

        private void ReportServiceBusQueueLength<TFlow, TTelemetryIdentity>()
            where TFlow : MessageFlowBase<TFlow>, new()
            where TTelemetryIdentity : TelemetryIdentityBase<TTelemetryIdentity>, new()
        {
            var flow = MessageFlowBase<TFlow>.Instance;
            var settings = _serviceBusSettingsFactory.CreateReceiverSettings(flow);
            var manager = NamespaceManager.CreateFromConnectionString(settings.ConnectionString);
            var subscription = manager.GetSubscription(settings.TransportEntityPath, flow.Id.ToString());
            _telemetry.Publish<TTelemetryIdentity>(subscription.MessageCountDetails.ActiveMessageCount);
        }

        private void ReportKafkaOffset<TFlow, TTelemetryIdentity>()
            where TFlow : MessageFlowBase<TFlow>, new()
            where TTelemetryIdentity : TelemetryIdentityBase<TTelemetryIdentity>, new()
        {
            var flow = MessageFlowBase<TFlow>.Instance;

            var size = _kafkaMessageFlowInfoProvider.GetFlowSize(flow);
            var processedSize = _kafkaMessageFlowInfoProvider.GetFlowProcessedSize(flow);

            _telemetry.Publish<TTelemetryIdentity>(size - processedSize);
        }

        private sealed class MessagesQueueLengthIdentity : TelemetryIdentityBase<MessagesQueueLengthIdentity>
        {
            public override int Id => 0;
            public override string Description => nameof(MessagesQueueLengthIdentity);
        }

        private sealed class AmsFactsQueueLengthIdentity : TelemetryIdentityBase<AmsFactsQueueLengthIdentity>
        {
            public override int Id => 0;

            public override string Description => nameof(AmsFactsQueueLengthIdentity);
        }

    }
}