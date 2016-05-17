using System;
using System.Collections.Generic;

using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Commands;
using NuClear.Replication.OperationsProcessing;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Final
{
    public sealed class CommonEventsAccumulator :
        MessageProcessingContextAccumulatorBase<CommonEventsFlow, EventMessage, AggregatableMessage<IAggregateCommand>>
    {
        private static readonly IReadOnlyDictionary<Type, Type> AggregateRoots
            = new Dictionary<Type, Type>
                {
                    { typeof(Storage.Model.Facts.Firm), typeof(Storage.Model.CI.Firm) },
                    { typeof(Storage.Model.Facts.Client), typeof(Storage.Model.CI.Client) },
                    { typeof(Storage.Model.Facts.Project), typeof(Storage.Model.CI.Project) },
                    { typeof(Storage.Model.Facts.Territory), typeof(Storage.Model.CI.Territory) },
                    { typeof(Storage.Model.Facts.CategoryGroup), typeof(Storage.Model.CI.CategoryGroup) },
                };

        protected override AggregatableMessage<IAggregateCommand> Process(EventMessage message)
        {
            return new AggregatableMessage<IAggregateCommand>
                {
                    TargetFlow = MessageFlow,
                    Commands = CreateCommands(message.Event),
                    EventHappenedTime = message.Event.Time,
                };
        }

        private static IReadOnlyCollection<IAggregateCommand> CreateCommands(IEvent @event)
        {
            var createdEvent = @event as DataObjectCreatedEvent;
            if (createdEvent != null)
            {
                return new [] { new InitializeAggregateCommand(AggregateRoots[createdEvent.DataObjectType], createdEvent.DataObjectId) };
            }

            var updatedEvent = @event as DataObjectUpdatedEvent;
            if (updatedEvent != null)
            {
                return new [] { new RecalculateAggregateCommand(AggregateRoots[updatedEvent.DataObjectType], updatedEvent.DataObjectId) };
            }

            var deletedEvent = @event as DataObjectDeletedEvent;
            if (deletedEvent != null)
            {
                return new [] { new DestroyAggregateCommand(AggregateRoots[deletedEvent.DataObjectType], deletedEvent.DataObjectId) };
            }

            var outdatedEvent = @event as RelatedDataObjectOutdatedEvent<long>;
            if (outdatedEvent != null)
            {
                return new [] { new RecalculateAggregateCommand(AggregateRoots[outdatedEvent.RelatedDataObjectType], outdatedEvent.RelatedDataObjectId) };
            }

            throw new ArgumentException($"Unexpected event '{@event}'", nameof(@event));
        }
    }
}