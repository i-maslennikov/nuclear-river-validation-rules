using System;

using Microsoft.ServiceBus;

using NuClear.Jobs;
using NuClear.Messaging.API.Flows;
using NuClear.Replication.OperationsProcessing.Telemetry;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus.Factories;
using NuClear.Security.API.Auth;
using NuClear.Security.API.Context;
using NuClear.Telemetry;
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;
using NuClear.ValidationRules.OperationsProcessing.AggregatesFlow;
using NuClear.ValidationRules.OperationsProcessing.FactsFlow;
using NuClear.ValidationRules.OperationsProcessing.MessagesFlow;
using NuClear.ValidationRules.OperationsProcessing.Telemetry;

using Quartz;

namespace NuClear.ValidationRules.Replication.Host.Jobs
{
    [DisallowConcurrentExecution]
    public sealed class ReportingJob : TaskServiceJobBase
    {
        private readonly ITelemetryPublisher _telemetry;
        private readonly IServiceBusSettingsFactory _serviceBusSettingsFactory;
        private readonly ITracer _tracer;

        public ReportingJob(
            ITelemetryPublisher telemetry,
            IServiceBusSettingsFactory serviceBusSettingsFactory,
            IUserContextManager userContextManager,
            IUserAuthenticationService userAuthenticationService,
            IUserAuthorizationService userAuthorizationService,
            ITracer tracer)
            : base(userContextManager, userAuthenticationService, userAuthorizationService, tracer)
        {
            _tracer = tracer;
            _telemetry = telemetry;
            _serviceBusSettingsFactory = serviceBusSettingsFactory;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            WithinErrorLogging(ReportMemoryUsage);
            WithinErrorLogging(ReportQueueLength<FactsFlow, PrimaryProcessingQueueLengthIdentity>);
            WithinErrorLogging(ReportQueueLength<AggregatesFlow, FinalProcessingAggregateQueueLengthIdentity>);
            WithinErrorLogging(ReportQueueLength<MessagesFlow, MessagesQueueLengthIdentity>);
            WithinErrorLogging(ReportProbes);
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

        private void ReportProbes()
        {
            var reports = DefaultReportSink.Instance.ConsumeReports();
            foreach (var report in reports)
            {
                _telemetry.Trace("ProbeReport", report);
            }
        }

        private void ReportQueueLength<TFlow, TTelemetryIdentity>()
            where TFlow : MessageFlowBase<TFlow>, new()
            where TTelemetryIdentity : TelemetryIdentityBase<TTelemetryIdentity>, new()
        {
            var flow = MessageFlowBase<TFlow>.Instance;
            var settings = _serviceBusSettingsFactory.CreateReceiverSettings(flow);
            var manager = NamespaceManager.CreateFromConnectionString(settings.ConnectionString);
            var subscription = manager.GetSubscription(settings.TransportEntityPath, flow.Id.ToString());
            _telemetry.Publish<TTelemetryIdentity>(subscription.MessageCountDetails.ActiveMessageCount);
        }
    }
}