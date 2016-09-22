﻿using System;
using System.Collections.Generic;

using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.ValidationRules.OperationsProcessing.Identities.Flows;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.OperationsProcessing.Final
{
    public sealed class AggregateCommandsAccumulator :
        MessageProcessingContextAccumulatorBase<CommonEventsFlow, EventMessage, AggregatableMessage<ICommand>>
    {
        private static readonly IReadOnlyDictionary<Type, Type> AggregateRoots
            = new Dictionary<Type, Type>
                {
                    { typeof(Storage.Model.PriceRules.Facts.Project), typeof(Storage.Model.PriceRules.Aggregates.Project) },
                    { typeof(Storage.Model.PriceRules.Facts.Price), typeof(Storage.Model.PriceRules.Aggregates.Price) },
                    { typeof(Storage.Model.PriceRules.Facts.Order), typeof(Storage.Model.PriceRules.Aggregates.Order) },
                    { typeof(Storage.Model.PriceRules.Facts.Position), typeof(Storage.Model.PriceRules.Aggregates.Position) },

                    { typeof(Storage.Model.ConsistencyRules.Facts.Order), typeof(Storage.Model.ConsistencyRules.Aggregates.Order) },

                    { typeof(Storage.Model.AccountRules.Facts.Account), typeof(Storage.Model.AccountRules.Aggregates.Account) },
                    { typeof(Storage.Model.AccountRules.Facts.Order), typeof(Storage.Model.AccountRules.Aggregates.Order) },
                };

        protected override AggregatableMessage<ICommand> Process(EventMessage message)
        {
            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = CreateCommands(message.Event),
            };
        }

        private IReadOnlyCollection<ICommand> CreateCommands(IEvent @event)
        {
            var createdEvent = @event as DataObjectCreatedEvent;
            if (createdEvent != null)
            {
                return new[] { new InitializeAggregateCommand(AggregateRoots[createdEvent.DataObjectType], createdEvent.DataObjectId) };
            }

            var updatedEvent = @event as DataObjectUpdatedEvent;
            if (updatedEvent != null)
            {
                return new[] { new RecalculateAggregateCommand(AggregateRoots[updatedEvent.DataObjectType], updatedEvent.DataObjectId) };
            }

            var deletedEvent = @event as DataObjectDeletedEvent;
            if (deletedEvent != null)
            {
                return new[] { new DestroyAggregateCommand(AggregateRoots[deletedEvent.DataObjectType], deletedEvent.DataObjectId) };
            }

            var outdatedEvent = @event as RelatedDataObjectOutdatedEvent<long>;
            if (outdatedEvent != null)
            {
                return new[] { new RecalculateAggregateCommand(AggregateRoots[outdatedEvent.RelatedDataObjectType], outdatedEvent.RelatedDataObjectId) };
            }

            var outdatedPeriodEvent = @event as RelatedDataObjectOutdatedEvent<PeriodKey>;
            if (outdatedPeriodEvent != null)
            {
                return new[] { new RecalculatePeriodAggregateCommand(outdatedPeriodEvent.RelatedDataObjectId) };
            }

            var stateIncrementedEvent = @event as FactsStateIncrementedEvent;
            if (stateIncrementedEvent != null)
            {
                return new[] { new IncrementStateCommand(stateIncrementedEvent.IncludedTokens) };
            }

            var batchProcessedEvent = @event as FactsBatchProcessedEvent;
            if (batchProcessedEvent != null)
            {
                return new[] { new RecordDelayCommand(batchProcessedEvent.EventTime) };
            }

            throw new ArgumentException($"Unexpected event '{@event}'", nameof(@event));
        }
    }
}