using System;

using NuClear.Jobs;
using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.API.Processing.Processors;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Metamodeling.Provider;
using NuClear.OperationsProcessing.API;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.OperationsProcessing.API.Primary;
using NuClear.Security.API.Context;
using NuClear.Security.API.Auth;
using NuClear.Telemetry;
using NuClear.Tracing.API;
using NuClear.Telemetry.Probing;
using NuClear.Utils;

using Quartz;

namespace NuClear.ValidationRules.Replication.Host.Jobs
{
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class ProcessingJob : TaskServiceJobBase
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly IMessageFlowProcessorFactory _messageFlowProcessorFactory;
        private readonly ITelemetryPublisher _telemetry;

        public ProcessingJob(
            IMetadataProvider metadataProvider,
            IMessageFlowProcessorFactory messageFlowProcessorFactory,
            IUserContextManager userContextManager,
            IUserAuthenticationService userAuthenticationService,
            IUserAuthorizationService userAuthorizationService,
            ITelemetryPublisher telemetry,
            ITracer tracer)
            : base(userContextManager, userAuthenticationService, userAuthorizationService, tracer)
        {
            _metadataProvider = metadataProvider;
            _messageFlowProcessorFactory = messageFlowProcessorFactory;
            _telemetry = telemetry;
        }

        public int FailCount { get; set; }
        public int BatchSize { get; set; }
        public string Flow { get; set; }

        private int BatchSizeAccordingFailures
            => Math.Max(BatchSize >> FailCount, 1);

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            if (string.IsNullOrEmpty(Flow))
            {
                string msg = string.Format("Required job arg {0} is not specified, check job config", StaticReflection.GetMemberName(() => Flow));
                Tracer.Fatal(msg);
                throw new InvalidOperationException(msg);
            }

            try
            {
                using (Probe.Create(Flow))
                {
                    ProcessFlow();
                }

                DecrementFailCount(context);
            }
            catch (Exception)
            {
                IncrementFailCount(context);
            }
            finally
            {
                var reports = DefaultReportSink.Instance.ConsumeReports();
                foreach (var report in reports)
                {
                    _telemetry.Trace("ProbeReport", report);
                }
            }
        }

        private void IncrementFailCount(IJobExecutionContext context)
        {
            context.JobDetail.JobDataMap.Put("FailCount", Math.Min(FailCount + 1, 31));
        }

        private void DecrementFailCount(IJobExecutionContext context)
        {
            context.JobDetail.JobDataMap.Put("FailCount", Math.Max(FailCount - 1, 0));
        }

        private void ProcessFlow()
        {
            MessageFlowMetadata messageFlowMetadata;
            if (!_metadataProvider.TryGetMetadata(Flow.AsPrimaryProcessingFlowId(), out messageFlowMetadata))
            {
                string msg = "Unsupported flow specified for processing: " + Flow;
                Tracer.Fatal(msg);
                throw new InvalidOperationException(msg);
            }

            Tracer.Debug("Launching message flow processing. Target message flow: " + messageFlowMetadata);

            ISyncMessageFlowProcessor messageFlowProcessor;

            try
            {
                var processorSettings = new PerformedOperationsPrimaryFlowProcessorSettings
                {
                    MessageBatchSize = BatchSizeAccordingFailures,
                    AppropriatedStages = new[]
                            {
                                MessageProcessingStage.Transformation,
                                MessageProcessingStage.Accumulation,
                                MessageProcessingStage.Handling
                            },
                    FirstFaultTolerantStage = MessageProcessingStage.None
                };

                messageFlowProcessor = _messageFlowProcessorFactory.CreateSync<IPerformedOperationsFlowProcessorSettings>(messageFlowMetadata, processorSettings);
            }
            catch (Exception ex)
            {
                Tracer.Error(ex, "Can't create processor for specified flow " + messageFlowMetadata);
                throw;
            }

            try
            {
                Tracer.Debug("Message flow processor starting. Target message flow: " + messageFlowMetadata);
                messageFlowProcessor.Process();
                Tracer.Debug("Message flow processor finished. Target message flow: " + messageFlowMetadata);
            }
            catch (Exception ex)
            {
                Tracer.Fatal(ex, "Message flow processor unexpectedly interrupted. Target message flow: " + messageFlowMetadata);
                throw;
            }
            finally
            {
                if (messageFlowProcessor != null)
                {
                    messageFlowProcessor.Dispose();
                }
            }
        }
    }
}