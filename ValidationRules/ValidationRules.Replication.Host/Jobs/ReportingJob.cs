using System;
using System.Data;
using System.Data.SqlClient;

using Microsoft.ServiceBus;

using NuClear.Jobs;
using NuClear.Messaging.Transports.ServiceBus.API;
using NuClear.Replication.OperationsProcessing.Telemetry;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.River.Hosting.Common.Identities.Connections;
using NuClear.Security.API;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.Telemetry;
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;

using Quartz;

namespace NuClear.ValidationRules.Replication.Host.Jobs
{
    [DisallowConcurrentExecution]
    public sealed class ReportingJob : TaskServiceJobBase
    {
        private readonly ITelemetryPublisher _telemetry;
        private readonly IServiceBusMessageReceiverSettings _serviceBusMessageReceiverSettings;
        private readonly IFlowLengthReporter _flowLengthReporter;
        private readonly NamespaceManager _manager;
        private readonly SqlConnection _sqlConnection;

        public ReportingJob(ITracer tracer,
                            ISignInService signInService,
                            IUserImpersonationService userImpersonationService,
                            ITelemetryPublisher telemetry,
                            IConnectionStringSettings connectionStringSettings,
                            IServiceBusMessageReceiverSettings serviceBusMessageReceiverSettings,
                            IFlowLengthReporter flowLengthReporter)
            : base(signInService, userImpersonationService, tracer)
        {
            _telemetry = telemetry;
            _serviceBusMessageReceiverSettings = serviceBusMessageReceiverSettings;
            _flowLengthReporter = flowLengthReporter;
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

            foreach (var flow in _flowLengthReporter.SqlFlows)
            {
                command.Parameters["@flowId"].Value = flow.Id;
                _flowLengthReporter.ReportFlowLength(flow, (int)command.ExecuteScalar());
            }

            _sqlConnection.Close();
        }

        private void ReportPrimaryProcessingQueueLength()
        {
            foreach (var flow in _flowLengthReporter.SeriviceBusFlows)
            {
                var subscription = _manager.GetSubscription(_serviceBusMessageReceiverSettings.TransportEntityPath, flow.Id.ToString());
                _flowLengthReporter.ReportFlowLength(flow, (int)subscription.MessageCountDetails.ActiveMessageCount);
            }
        }
    }
}