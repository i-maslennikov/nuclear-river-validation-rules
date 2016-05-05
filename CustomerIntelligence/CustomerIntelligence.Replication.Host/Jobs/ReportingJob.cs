using System;
using System.Data;
using System.Data.SqlClient;

using Microsoft.ServiceBus;

using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.Jobs;
using NuClear.Messaging.Transports.ServiceBus.API;
using NuClear.Replication.OperationsProcessing.Telemetry;
using NuClear.River.Hosting.Common.Identities.Connections;
using NuClear.Security.API;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.Telemetry;
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;

using Quartz;

namespace NuClear.CustomerIntelligence.Replication.Host.Jobs
{
    [DisallowConcurrentExecution]
    public sealed class ReportingJob : TaskServiceJobBase
    {
        private readonly ITelemetryPublisher _telemetry;
        private readonly IServiceBusMessageReceiverSettings _serviceBusMessageReceiverSettings;
        private readonly NamespaceManager _manager;
        private readonly SqlConnection _sqlConnection;

        public ReportingJob(ITracer tracer,
                            ISignInService signInService,
                            IUserImpersonationService userImpersonationService,
                            ITelemetryPublisher telemetry,
                            IConnectionStringSettings connectionStringSettings,
                            IServiceBusMessageReceiverSettings serviceBusMessageReceiverSettings)
            : base(signInService, userImpersonationService, tracer)
        {
            _telemetry = telemetry;
            _serviceBusMessageReceiverSettings = serviceBusMessageReceiverSettings;
            _manager = NamespaceManager.CreateFromConnectionString(connectionStringSettings.GetConnectionString(ServiceBusConnectionStringIdentity.Instance));
            _sqlConnection = new SqlConnection(connectionStringSettings.GetConnectionString(TransportConnectionStringIdentity.Instance));
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            ReportMemoryUsage();
            ReportPrimaryProcessingQueueLength();
            ReportFinalProcessingQueueLength();
            ReportProbes();
        }

        private void ReportMemoryUsage()
        {
            try
            {
                var process = System.Diagnostics.Process.GetCurrentProcess();
                _telemetry.Publish<ProcessPrivateMemorySizeIdentity>(process.PrivateMemorySize64);
                _telemetry.Publish<ProcessWorkingSetIdentity>(process.WorkingSet64);
            }
            catch (Exception)
            {
            }
        }

        private void ReportProbes()
        {
            var reports = DefaultReportSink.Instance.ConsumeReports();
            foreach (var report in reports)
            {
                _telemetry.Trace("ProbeReport", report);
            }
        }

        private void ReportFinalProcessingQueueLength()
        {
            if (_sqlConnection.State != ConnectionState.Open)
            {
                _sqlConnection.Open();
            }

            const string CommandText = "select count(*) from Transport.PerformedOperationFinalProcessing " +
                                       "where MessageFlowId = @flowId";
            var command = new SqlCommand(CommandText, _sqlConnection);
            command.Parameters.Add("@flowId", SqlDbType.UniqueIdentifier);

            command.Parameters["@flowId"].Value = CommonEventsFlow.Instance.Id;
            _telemetry.Publish<FinalProcessingAggregateQueueLengthIdentity>((int)command.ExecuteScalar());

            command.Parameters["@flowId"].Value = StatisticsEventsFlow.Instance.Id;
            _telemetry.Publish<FinalProcessingStatisticsQueueLengthIdentity>((int)command.ExecuteScalar());

            _sqlConnection.Close();
        }

        private void ReportPrimaryProcessingQueueLength()
        {
            var subscription = _manager.GetSubscription(_serviceBusMessageReceiverSettings.TransportEntityPath, ImportFactsFromErmFlow.Instance.Id.ToString());
            _telemetry.Publish<PrimaryProcessingQueueLengthIdentity>(subscription.MessageCountDetails.ActiveMessageCount);
        }
    }
}