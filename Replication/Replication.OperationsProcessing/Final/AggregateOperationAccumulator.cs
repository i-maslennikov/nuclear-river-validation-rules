using System.Linq;

using NuClear.AdvancedSearch.Common.Metadata.Model.Operations;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;

namespace NuClear.Replication.OperationsProcessing.Final
{
    public sealed class AggregateOperationAccumulator<TMessageFlow> :
        MessageProcessingContextAccumulatorBase<TMessageFlow, PerformedOperationsFinalProcessingMessage, OperationAggregatableMessage<AggregateOperation>>
        where TMessageFlow : class, IMessageFlow, new()
    {
        private readonly AggregateOperationSerializer _serializer;

        public AggregateOperationAccumulator(AggregateOperationSerializer serializer)
        {
            _serializer = serializer;
        }

        protected override OperationAggregatableMessage<AggregateOperation> Process(PerformedOperationsFinalProcessingMessage message)
        {
            var operations = message.FinalProcessings.Select(x => _serializer.Deserialize(x)).ToArray();
            var oldestOperation = message.FinalProcessings.Min(x => x.CreatedOn);

            return new OperationAggregatableMessage<AggregateOperation>
            {
                TargetFlow = MessageFlow,
                Operations = operations,
                OperationTime = oldestOperation,
            };
        }
    }
}