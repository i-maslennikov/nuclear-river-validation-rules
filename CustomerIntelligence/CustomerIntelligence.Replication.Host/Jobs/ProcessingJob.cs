using System;

using NuClear.Jobs;
using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.API.Processing.Processors;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Metamodeling.Provider;
using NuClear.OperationsProcessing.API;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.OperationsProcessing.API.Primary;
using NuClear.Security.API;
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;
using NuClear.Utils;

using Quartz;

namespace NuClear.CustomerIntelligence.Replication.Host.Jobs
{
    [DisallowConcurrentExecution]
    public class ProcessingJob : TaskServiceJobBase
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly IMessageFlowProcessorFactory _messageFlowProcessorFactory;

        public ProcessingJob(
            IMetadataProvider metadataProvider,
            IMessageFlowProcessorFactory messageFlowProcessorFactory,
            ISignInService signInService,
            IUserImpersonationService userImpersonationService,
            ITracer tracer)
            : base(signInService, userImpersonationService, tracer)
        {
            _metadataProvider = metadataProvider;
            _messageFlowProcessorFactory = messageFlowProcessorFactory;
        }

        public int BatchSize { get; set; }
        public string Flow { get; set; }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            if (string.IsNullOrEmpty(Flow))
            {
                string msg = string.Format("Required job arg {0} is not specified, check job config", StaticReflection.GetMemberName(() => Flow));
                Tracer.Fatal(msg);
                throw new InvalidOperationException(msg);
            }

            using (Probe.Create(Flow))
            {
                ProcessFlow();
            }
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
                        MessageBatchSize = BatchSize,
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