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
        protected override AggregatableMessage<ICommand> Process(EventMessage message)
        {
            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = CommandFactory.CreateCommands(message.Event).ToList()
            };
        }

        private static class CommandFactory
        {
            public static IEnumerable<ICommand> CreateCommands(IEvent @event)
            {
                var createdEvent = @event as DataObjectCreatedEvent;
                if (createdEvent != null)
                {
                    return AggregateTypesFor<DataObjectCreatedEvent>(createdEvent.DataObjectType)
                           .Select(x => new AggregateCommand.Recalculate(x, createdEvent.DataObjectId));
                }

                var updatedEvent = @event as DataObjectUpdatedEvent;
                if (updatedEvent != null)
                {
                    return AggregateTypesFor<DataObjectUpdatedEvent>(updatedEvent.DataObjectType)
                           .Select(x => new AggregateCommand.Recalculate(x, updatedEvent.DataObjectId));
                }

                var deletedEvent = @event as DataObjectDeletedEvent;
                if (deletedEvent != null)
                {
                    return AggregateTypesFor<DataObjectDeletedEvent>(deletedEvent.DataObjectType)
                           .Select(x => new AggregateCommand.Recalculate(x, deletedEvent.DataObjectId));
                }

                var outdatedEvent = @event as RelatedDataObjectOutdatedEvent<long>;
                if (outdatedEvent != null)
                {
                    return RelatedAggregateTypesFor<RelatedDataObjectOutdatedEvent<long>>(outdatedEvent.DataObjectType, outdatedEvent.RelatedDataObjectType)
                           .Select(x => new AggregateCommand.Recalculate(x, outdatedEvent.RelatedDataObjectId));
                }

                var outdatedPeriodEvent = @event as RelatedDataObjectOutdatedEvent<PeriodKey>;
                if (outdatedPeriodEvent != null)
                {
                    return new[] { new RecalculatePeriodCommand(outdatedPeriodEvent.RelatedDataObjectId) };
                }

                var stateIncrementedEvent = @event as FactsStateIncrementedEvent;
                if (stateIncrementedEvent != null)
                {
                    return new[] { new IncrementStateCommand(stateIncrementedEvent.IncludedTokens) };
                }

                var factsDelayLoggedEvent = @event as FactsDelayLoggedEvent;
                if (factsDelayLoggedEvent != null)
                {
                    return new[] { new LogDelayCommand(factsDelayLoggedEvent.EventTime) };
                }

                throw new ArgumentException($"Unexpected event '{@event}'", nameof(@event));
            }

            private static IEnumerable<Type> AggregateTypesFor<TEvent>(Type dataObjectType)
                where TEvent : IEvent
            {
                IReadOnlyCollection<Type> aggregateTypes;
                if (!EntityTypeMap.TryGetAggregateTypes(dataObjectType, out aggregateTypes))
                {
                    throw new ArgumentException($"No metadata for event {typeof(TEvent).Name}, DataObjectType={dataObjectType.Name}", nameof(dataObjectType));
                }

                return aggregateTypes;
            }

            private static IEnumerable<Type> RelatedAggregateTypesFor<TEvent>(Type dataObjectType, Type relatedDataObjectType)
                where TEvent : IEvent
            {
                IReadOnlyCollection<Type> aggregateTypes;
                if (!EntityTypeMap.TryGetRelatedAggregateTypes(dataObjectType, relatedDataObjectType, out aggregateTypes))
                {
                    throw new ArgumentException($"No metadata for event {typeof(TEvent).GetFriendlyName() } ({dataObjectType.Name}, {relatedDataObjectType.Name})", nameof(dataObjectType));
                }

                return aggregateTypes;
            }
        }
    }
}