using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Tracing.API;
using NuClear.ValidationRules.OperationsProcessing.Identities.Flows;

namespace NuClear.ValidationRules.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromErmHandler : IMessageProcessingHandler
    {
        private readonly IFactsReplicator _factsReplicator;
        private readonly IOperationSender _aggregateSender;
        private readonly ITracer _tracer;

        public ImportFactsFromErmHandler(
            IFactsReplicator factsReplicator,
            IOperationSender aggregateSender,
            ITracer tracer)
        {
            _aggregateSender = aggregateSender;
            _tracer = tracer;
            _factsReplicator = factsReplicator;
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            try
            {
                var messages = processingResultsMap.SelectMany(pair => pair.Value)
                                                   .Cast<OperationAggregatableMessage<FactOperation>>()
                                                   .ToArray();

                Handle(messages.SelectMany(message => message.Operations).ToArray());

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
            var result = _factsReplicator.Replicate(operations);
            var aggregates = result.OfType<AggregateOperation>().ToArray();

            // We always need to use different transaction scope to operate with operation sender because it has its own store
            using (var pushTransaction = new TransactionScope(TransactionScopeOption.RequiresNew,
                                                              new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
            {
                _tracer.Debug("Pushing events for aggregates recalculation");
                _aggregateSender.Push(aggregates, AggregatesFlow.Instance);

                pushTransaction.Complete();
            }
        }
    }
}
