using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.OperationsProcessing.Transports.SQLStore;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing;
using NuClear.River.Common.Metadata.Model.Operations;
using System.Collections.Generic;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Final
{
    public sealed class AggregateOperationAccumulator<TMessageFlow> :
        MessageProcessingContextAccumulatorBase<TMessageFlow, PerformedOperationsFinalProcessingMessage, OperationAggregatableMessage<IAggregateCommand>>
        where TMessageFlow : class, IMessageFlow, new()
    {
        private readonly AggregateOperationSerializer _serializer;

        public AggregateOperationAccumulator(AggregateOperationSerializer serializer)
        {
            _serializer = serializer;
        }

        protected override OperationAggregatableMessage<IAggregateCommand> Process(PerformedOperationsFinalProcessingMessage message)
        {
            var events = message.FinalProcessings.Select(x => _serializer.Deserialize(x)).ToArray();
            var commands = new List<IAggregateCommand>();
            foreach (var @event in events)
            {
                var eventType = @event.GetType();
                if (eventType == typeof(InitializeAggregate))
                {
                    commands.Add(new InitializeAggregateCommand(@event.AggregateType, @event.AggregateId));
                }

                if (eventType == typeof(RecalculateAggregate))
                {
                    commands.Add(new RecalculateAggregateCommand(@event.AggregateType, @event.AggregateId));
                }

                if (eventType == typeof(RecalculateAggregate))
                {
                    commands.Add(new RecalculateAggregateCommand(@event.AggregateType, @event.AggregateId));
                }
            }

            var oldestOperation = message.FinalProcessings.Min(x => x.CreatedOn);

            return new OperationAggregatableMessage<IAggregateCommand>
            {
                TargetFlow = MessageFlow,
                Commands = commands,
                OperationTime = oldestOperation,
            };
        }
    }
}