using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.OperationsProcessing.AggregatesFlow
{
    public sealed class AggregatesFlowAccumulator : MessageProcessingContextAccumulatorBase<AggregatesFlow, EventMessage, AggregatableMessage<ICommand>>
    {
        private readonly ICommandFactory _commandFactory;

        public AggregatesFlowAccumulator()
        {
            _commandFactory = new AggregatesCommandFactory();
        }

        protected override AggregatableMessage<ICommand> Process(EventMessage message)
        {
            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = _commandFactory.CreateCommands(message.Event).ToList()
            };
        }

        private sealed class AggregatesCommandFactory : ICommandFactory
        {
            public IEnumerable<ICommand> CreateCommands(IEvent @event)
            {
                switch (@event)
                {
                    case DataObjectCreatedEvent createdEvent:
                        return AggregateTypesFor<DataObjectCreatedEvent>(createdEvent.DataObjectType)
                            .Select(x => new AggregateCommand.Recalculate(x, createdEvent.DataObjectId));

                    case DataObjectUpdatedEvent updatedEvent:
                        return AggregateTypesFor<DataObjectUpdatedEvent>(updatedEvent.DataObjectType)
                            .Select(x => new AggregateCommand.Recalculate(x, updatedEvent.DataObjectId));

                    case DataObjectDeletedEvent deletedEvent:
                        return AggregateTypesFor<DataObjectDeletedEvent>(deletedEvent.DataObjectType)
                            .Select(x => new AggregateCommand.Recalculate(x, deletedEvent.DataObjectId));

                    case RelatedDataObjectOutdatedEvent<long> outdatedEvent:
                        return RelatedAggregateTypesFor<RelatedDataObjectOutdatedEvent<long>>(outdatedEvent.DataObjectType, outdatedEvent.RelatedDataObjectType)
                            .Select(x => new AggregateCommand.Recalculate(x, outdatedEvent.RelatedDataObjectId));

                    case RelatedDataObjectOutdatedEvent<PeriodKey> outdatedPeriodEvent:
                        return new[] { new RecalculatePeriodCommand(outdatedPeriodEvent.RelatedDataObjectId) };

                    case AmsStateIncrementedEvent amsStateIncrementedEvent:
                        return new[] { new IncrementAmsStateCommand(amsStateIncrementedEvent.State) };

                    case ErmStateIncrementedEvent ermStateIncrementedEvent:
                        return new[] { new IncrementErmStateCommand(ermStateIncrementedEvent.States) };

                    case DelayLoggedEvent delayLoggedEvent:
                        return new[] { new LogDelayCommand(delayLoggedEvent.EventTime) };

                    default:
                        throw new ArgumentException($"Unexpected event '{@event}'", nameof(@event));
                }
            }

            private static IEnumerable<Type> AggregateTypesFor<TEvent>(Type dataObjectType)
                where TEvent : IEvent
            {
                if (!EntityTypeMap.TryGetAggregateTypes(dataObjectType, out var aggregateTypes))
                {
                    throw new ArgumentException($"No metadata for event {typeof(TEvent).Name}, DataObjectType={dataObjectType.Name}", nameof(dataObjectType));
                }

                return aggregateTypes;
            }

            private static IEnumerable<Type> RelatedAggregateTypesFor<TEvent>(Type dataObjectType, Type relatedDataObjectType)
                where TEvent : IEvent
            {
                if (!EntityTypeMap.TryGetRelatedAggregateTypes(dataObjectType, relatedDataObjectType, out var aggregateTypes))
                {
                    throw new ArgumentException($"No metadata for event {typeof(TEvent).GetFriendlyName() } ({dataObjectType.Name}, {relatedDataObjectType.Name})", nameof(dataObjectType));
                }

                return aggregateTypes;
            }
        }
    }
}