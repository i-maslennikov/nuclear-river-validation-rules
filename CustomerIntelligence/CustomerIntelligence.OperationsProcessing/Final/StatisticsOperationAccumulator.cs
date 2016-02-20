using System.Linq;

using NuClear.CustomerIntelligence.OperationsProcessing.Transports.SQLStore;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Final
{
    public sealed class StatisticsOperationAccumulator<TMessageFlow> :
        MessageProcessingContextAccumulatorBase<TMessageFlow, PerformedOperationsFinalProcessingMessage, OperationAggregatableMessage<RecalculateStatisticsOperation>>
        where TMessageFlow : class, IMessageFlow, new()
    {
        private readonly StatisticsOperationSerializer _serializer;

        public StatisticsOperationAccumulator(StatisticsOperationSerializer serializer)
        {
            _serializer = serializer;
        }

        protected override OperationAggregatableMessage<RecalculateStatisticsOperation> Process(PerformedOperationsFinalProcessingMessage message)
        {
            var operations = message.FinalProcessings.Select(x => _serializer.Deserialize(x)).ToArray();
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