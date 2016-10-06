using System;
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
                    { typeof(Storage.Model.PriceRules.Facts.Theme), typeof(Storage.Model.PriceRules.Aggregates.Theme) },
                    { typeof(Storage.Model.PriceRules.Facts.Category), typeof(Storage.Model.PriceRules.Aggregates.Category) },

                    { typeof(Storage.Model.ConsistencyRules.Facts.Order), typeof(Storage.Model.ConsistencyRules.Aggregates.Order) },

                    { typeof(Storage.Model.AccountRules.Facts.Account), typeof(Storage.Model.AccountRules.Aggregates.Account) },
                    { typeof(Storage.Model.AccountRules.Facts.Order), typeof(Storage.Model.AccountRules.Aggregates.Order) },

                    { typeof(Storage.Model.FirmRules.Facts.Firm), typeof(Storage.Model.FirmRules.Aggregates.Firm) },
                    { typeof(Storage.Model.FirmRules.Facts.Order), typeof(Storage.Model.FirmRules.Aggregates.Order) },

                    { typeof(Storage.Model.AdvertisementRules.Facts.Advertisement), typeof(Storage.Model.AdvertisementRules.Aggregates.Advertisement) },
                    { typeof(Storage.Model.AdvertisementRules.Facts.AdvertisementElementTemplate), typeof(Storage.Model.AdvertisementRules.Aggregates.AdvertisementElementTemplate) },
                    { typeof(Storage.Model.AdvertisementRules.Facts.Firm), typeof(Storage.Model.AdvertisementRules.Aggregates.Firm) },
                    { typeof(Storage.Model.AdvertisementRules.Facts.Order), typeof(Storage.Model.AdvertisementRules.Aggregates.Order) },
                    { typeof(Storage.Model.AdvertisementRules.Facts.Position), typeof(Storage.Model.AdvertisementRules.Aggregates.Position) },

                    { typeof(Storage.Model.ProjectRules.Facts.Project), typeof(Storage.Model.ProjectRules.Aggregates.Project) },
                    { typeof(Storage.Model.ProjectRules.Facts.FirmAddress), typeof(Storage.Model.ProjectRules.Aggregates.FirmAddress) },
                    { typeof(Storage.Model.ProjectRules.Facts.Order), typeof(Storage.Model.ProjectRules.Aggregates.Order) },
                    { typeof(Storage.Model.ProjectRules.Facts.Position), typeof(Storage.Model.ProjectRules.Aggregates.Position) },
                    { typeof(Storage.Model.ProjectRules.Facts.Category), typeof(Storage.Model.ProjectRules.Aggregates.Category) },
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
                return new[] { new InitializeAggregateCommand(MapDataObjectToAggregate(createdEvent.DataObjectType), createdEvent.DataObjectId) };
            }

            var updatedEvent = @event as DataObjectUpdatedEvent;
            if (updatedEvent != null)
            {
                return new[] { new RecalculateAggregateCommand(MapDataObjectToAggregate(updatedEvent.DataObjectType), updatedEvent.DataObjectId) };
            }

            var deletedEvent = @event as DataObjectDeletedEvent;
            if (deletedEvent != null)
            {
                return new[] { new DestroyAggregateCommand(MapDataObjectToAggregate(deletedEvent.DataObjectType), deletedEvent.DataObjectId) };
            }

            var outdatedEvent = @event as RelatedDataObjectOutdatedEvent<long>;
            if (outdatedEvent != null)
            {
                return new[] { new RecalculateAggregateCommand(MapDataObjectToAggregate(outdatedEvent.RelatedDataObjectType), outdatedEvent.RelatedDataObjectId) };
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

        private Type MapDataObjectToAggregate(Type dataObjectType)
        {
            Type result;
            if (!AggregateRoots.TryGetValue(dataObjectType, out result))
            {
                throw new ArgumentException($"Type {dataObjectType.FullName} is not mapped with aggregate root type", nameof(dataObjectType));
            }

            return result;
        }
    }
}