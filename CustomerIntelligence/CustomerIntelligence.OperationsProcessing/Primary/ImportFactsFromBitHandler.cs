using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.OperationsProcessing.Identities.Telemetry;
using NuClear.Replication.OperationsProcessing.Primary;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.River.Common.Metadata.Model;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromBitHandler : IMessageProcessingHandler
    {
        private readonly IImportDocumentMetadataProcessorFactory _importDocumentMetadataProcessorFactory;
        private readonly IOperationSender _sender;
        private readonly IOperationDispatcher _operationDispatcher;
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public ImportFactsFromBitHandler(
            IImportDocumentMetadataProcessorFactory importDocumentMetadataProcessorFactory,
            IOperationSender sender,
            ITelemetryPublisher telemetryPublisher,
            IOperationDispatcher operationDispatcher,
            ITracer tracer)
        {
            _importDocumentMetadataProcessorFactory = importDocumentMetadataProcessorFactory;
            _sender = sender;
            _tracer = tracer;
            _telemetryPublisher = telemetryPublisher;
            _operationDispatcher = operationDispatcher;
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            return processingResultsMap.Select(pair => Handle(pair.Key, pair.Value));
        }

        private StageResult Handle(Guid bucketId, IEnumerable<IAggregatableMessage> messages)
        {
            try
            {
                foreach (var message in messages.OfType<CorporateBusAggregatableMessage>())
                {
                    foreach (var dto in message.Dtos)
                    {
                        var importer = _importDocumentMetadataProcessorFactory.Create(dto.GetType());
                        var opertaions = importer.Import(dto);
                        _telemetryPublisher.Publish<BitStatisticsEntityProcessedCountIdentity>(1);
                        DispatchOperations(opertaions);
                    }
                }

                return MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error when import facts for BIT");
                return MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }

        private void DispatchOperations(IEnumerable<IOperation> opertaions)
        {
            foreach (var pair in _operationDispatcher.Dispatch(opertaions))
            {
                _sender.Push(pair.Value, pair.Key);
            }
        }
    }
}