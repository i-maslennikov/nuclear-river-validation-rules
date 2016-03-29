using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Identities.Telemetry;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromErmHandler : IMessageProcessingHandler
    {
        private readonly IFactsReplicator _factsReplicator;
        private readonly IOperationSender _operationSender;
        private readonly IOperationDispatcher _operationDispatcher;
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public ImportFactsFromErmHandler(
            IFactsReplicator factsReplicator,
            IOperationSender operationSender,
            ITelemetryPublisher telemetryPublisher,
            IOperationDispatcher operationDispatcher,
            ITracer tracer)
        {
            _operationSender = operationSender;
            _telemetryPublisher = telemetryPublisher;
            _factsReplicator = factsReplicator;
            _operationDispatcher = operationDispatcher;
            _tracer = tracer;
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            try
            {
                var messages = processingResultsMap.SelectMany(pair => pair.Value)
                                                   .Cast<OperationAggregatableMessage<FactOperation>>()
                                                   .ToArray();

                Handle(messages.SelectMany(message => message.Operations).ToArray());

                var eldestOperationPerformTime = messages.Min(message => message.OperationTime);
                _telemetryPublisher.Publish<PrimaryProcessingDelayIdentity>((long)(DateTime.UtcNow - eldestOperationPerformTime).TotalMilliseconds);

                return processingResultsMap.Keys.Select(bucketId => MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded());
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error when import facts for ERM");
                return processingResultsMap.Keys.Select(bucketId => MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex));
            }
        }

        private void Handle(IReadOnlyCollection<FactOperation> operations)
        {
            _tracer.Debug("Handing fact operations started");
            var result = _factsReplicator.Replicate(operations);

            _telemetryPublisher.Publish<ErmProcessedOperationCountIdentity>(operations.Count);

            // We always need to use different transaction scope to operate with operation sender because it has its own store
            using (var pushTransaction = new TransactionScope(TransactionScopeOption.RequiresNew,
                                                              new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
            {
                _tracer.Debug("Pushing messages");
                DispatchOperations(result);
                pushTransaction.Complete();
            }

            _tracer.Debug("Handing fact operations finished");
        }

        private void DispatchOperations(IEnumerable<IOperation> opertaions)
        {
            var dispatched = _operationDispatcher.Dispatch(opertaions);
            foreach (var pair in dispatched)
            {
                _operationSender.Push(pair.Value, pair.Key);
            }

            _telemetryPublisher.Publish<StatisticsEnqueuedOperationCountIdentity>(dispatched[StatisticsFlow.Instance].Count);
            _telemetryPublisher.Publish<AggregateEnqueuedOperationCountIdentity>(dispatched[AggregatesFlow.Instance].Count);
        }
    }
}