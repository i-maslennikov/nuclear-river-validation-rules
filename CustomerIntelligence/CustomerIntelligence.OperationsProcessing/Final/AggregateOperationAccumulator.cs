using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Final
{
    public sealed class AggregateOperationAccumulator<TMessageFlow> :
        MessageProcessingContextAccumulatorBase<TMessageFlow, PerformedOperationsFinalProcessingMessage, OperationAggregatableMessage<IAggregateCommand>>
        where TMessageFlow : class, IMessageFlow, new()
    {
        private readonly IEventSerializer _serializer;

        public AggregateOperationAccumulator(IEventSerializer serializer)
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
                    commands.Add(new InitializeAggregateCommand(@event.AggregateRoot.EntityType, (long)@event.AggregateRoot.EntityKey));
                }
                else if (eventType == typeof(RecalculateAggregate))
                {
                    commands.Add(new RecalculateAggregateCommand(@event.AggregateType, @event.AggregateId));
                }
                else if (eventType == typeof(RecalculateAggregate))
                {
                    commands.Add(new RecalculateAggregateCommand(@event.AggregateType, @event.AggregateId));
                }
            }

            var oldestEvent = message.FinalProcessings.Min(x => x.CreatedOn);

            return new OperationAggregatableMessage<IAggregateCommand>
            {
                TargetFlow = MessageFlow,
                Commands = commands,
                OperationTime = oldestEvent,
            };
        }
    }
}