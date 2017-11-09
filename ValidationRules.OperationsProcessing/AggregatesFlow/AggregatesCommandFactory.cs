using System;
using System.Collections.Generic;
using System.Linq;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.OperationsProcessing.AggregatesFlow
{
    public sealed class AggregatesCommandFactory : ICommandFactory<EventMessage>
    {
        public IReadOnlyCollection<ICommand> CreateCommands(EventMessage message)
        {
            switch (message.Event)
            {
                case DataObjectCreatedEvent createdEvent:
                    return AggregateTypesFor<DataObjectCreatedEvent>(createdEvent.DataObjectType)
                        .Select(x => new AggregateCommand.Recalculate(x, createdEvent.DataObjectId))
                        .ToList();

                case DataObjectUpdatedEvent updatedEvent:
                    return AggregateTypesFor<DataObjectUpdatedEvent>(updatedEvent.DataObjectType)
                        .Select(x => new AggregateCommand.Recalculate(x, updatedEvent.DataObjectId))
                        .ToList();

                case DataObjectDeletedEvent deletedEvent:
                    return AggregateTypesFor<DataObjectDeletedEvent>(deletedEvent.DataObjectType)
                        .Select(x => new AggregateCommand.Recalculate(x, deletedEvent.DataObjectId))
                        .ToList();

                case RelatedDataObjectOutdatedEvent<long> outdatedEvent:
                    return RelatedAggregateTypesFor<RelatedDataObjectOutdatedEvent<long>>(outdatedEvent.DataObjectType, outdatedEvent.RelatedDataObjectType)
                        .Select(x => new AggregateCommand.Recalculate(x, outdatedEvent.RelatedDataObjectId))
                        .ToList();

                case RelatedDataObjectOutdatedEvent<PeriodKey> outdatedPeriodEvent:
                    return new[] { new RecalculatePeriodCommand(outdatedPeriodEvent.RelatedDataObjectId) };

                case AmsStateIncrementedEvent amsStateIncrementedEvent:
                    return new[] { new IncrementAmsStateCommand(amsStateIncrementedEvent.State) };

                case ErmStateIncrementedEvent ermStateIncrementedEvent:
                    return new[] { new IncrementErmStateCommand(ermStateIncrementedEvent.States) };

                case DelayLoggedEvent delayLoggedEvent:
                    return new[] { new LogDelayCommand(delayLoggedEvent.EventTime) };

                default:
                    throw new ArgumentException($"Unexpected event '{message}'", nameof(message));
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
