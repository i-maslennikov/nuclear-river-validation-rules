using System.Linq;

using NuClear.CustomerIntelligence.OperationsProcessing.Transports.SQLStore;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing;
using NuClear.River.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Final
{
    public sealed class AggregateOperationAccumulator<TMessageFlow> :
        MessageProcessingContextAccumulatorBase<TMessageFlow, PerformedOperationsFinalProcessingMessage, OperationAggregatableMessage<IOperation>>
        where TMessageFlow : class, IMessageFlow, new()
    {
        private readonly XmlOperationSerializer _serializer;

        public AggregateOperationAccumulator(XmlOperationSerializer serializer)
        {
            _serializer = serializer;
        }

        protected override OperationAggregatableMessage<IOperation> Process(PerformedOperationsFinalProcessingMessage message)
        {
            var operations = message.FinalProcessings.Select(x => _serializer.Deserialize(x)).ToArray();
            var oldestOperation = message.FinalProcessings.Min(x => x.CreatedOn);

            return new OperationAggregatableMessage<IOperation>
            {
                TargetFlow = MessageFlow,
                Operations = operations,
                OperationTime = oldestOperation,
            };
        }
    }
}