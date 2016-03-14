using System.Linq;

using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Final
{
    public sealed class StatisticsOperationAccumulator<TMessageFlow> :
        MessageProcessingContextAccumulatorBase<TMessageFlow, PerformedOperationsFinalProcessingMessage, OperationAggregatableMessage<RecalculateStatisticsOperation>>
        where TMessageFlow : class, IMessageFlow, new()
    {
        private readonly IOperationSerializer _serializer;

        public StatisticsOperationAccumulator(IOperationSerializer serializer)
        {
            _serializer = serializer;
        }

        protected override OperationAggregatableMessage<RecalculateStatisticsOperation> Process(PerformedOperationsFinalProcessingMessage message)
        {
            // todo: RecalculateStatisticsOperation -> IOperation
            var operations = message.FinalProcessings.Select(_serializer.Deserialize).Cast<RecalculateStatisticsOperation>().ToArray();
            var oldestOperation = message.FinalProcessings.Min(x => x.CreatedOn);

            return new OperationAggregatableMessage<RecalculateStatisticsOperation>
            {
                TargetFlow = MessageFlow,
                Operations = operations,
                OperationTime = oldestOperation,
            };
        }
    }
}