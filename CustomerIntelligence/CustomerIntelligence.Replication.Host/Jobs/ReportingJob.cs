using System;

using Microsoft.ServiceBus;

using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.Jobs;
using NuClear.Messaging.API.Flows;
using NuClear.Replication.OperationsProcessing.Telemetry;
using NuClear.Security.API;
using NuClear.Telemetry;
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;

using Quartz;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus.Factories;

namespace NuClear.CustomerIntelligence.Replication.Host.Jobs
{
    [DisallowConcurrentExecution]
    public sealed class ReportingJob : TaskServiceJobBase
    {
        private readonly ITelemetryPublisher _telemetry;
        private readonly IServiceBusSettingsFactory _serviceBusSettingsFactory;
        private readonly ITracer _tracer;

        public ReportingJob(ITracer tracer,
                            ISignInService signInService,
                            IUserImpersonationService userImpersonationService,
                            ITelemetryPublisher telemetry,
                            IServiceBusSettingsFactory serviceBusSettingsFactory)
            : base(signInService, userImpersonationService, tracer)
        {
            _tracer = tracer;
            _telemetry = telemetry;
            _serviceBusSettingsFactory = serviceBusSettingsFactory;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            WithinErrorLogging(ReportMemoryUsage);
            WithinErrorLogging(ReportQueueLength<ImportFactsFromErmFlow, PrimaryProcessingQueueLengthIdentity>);
            WithinErrorLogging(ReportQueueLength<CommonEventsFlow, FinalProcessingAggregateQueueLengthIdentity>);
            WithinErrorLogging(ReportQueueLength<StatisticsEventsFlow, FinalProcessingStatisticsQueueLengthIdentity>);
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