using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.ValidationRules.OperationsProcessing.Identities.Flows;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.OperationsProcessing.Final
{
    public sealed class AggregateCommandsAccumulator :
        MessageProcessingContextAccumulatorBase<CommonEventsFlow, EventMessage, AggregatableMessage<ICommand>>
    {
        private static readonly IReadOnlyDictionary<Type, IReadOnlyCollection<Type>> AggregateRoots
            = new Dictionary<Type, IReadOnlyCollection<Type>>
                {
                    { typeof(Order), new[]
                        {
                            typeof(Storage.Model.AccountRules.Aggregates.Order),
                            typeof(Storage.Model.ThemeRules.Aggregates.Order),
                            typeof(Storage.Model.AdvertisementRules.Aggregates.Order),
                            typeof(Storage.Model.FirmRules.Aggregates.Order),
                            typeof(Storage.Model.ConsistencyRules.Aggregates.Order),
                            typeof(Storage.Model.PriceRules.Aggregates.Order),
                            typeof(Storage.Model.ProjectRules.Aggregates.Order)
                        }
                    },
                    { typeof(Category), new []
                        {
                            typeof(Storage.Model.ThemeRules.Aggregates.Category),
                            typeof(Storage.Model.PriceRules.Aggregates.Category),
                            typeof(Storage.Model.ProjectRules.Aggregates.Category)
                        }
                    },
                    { typeof(Project), new []
                        {
                            typeof(Storage.Model.ThemeRules.Aggregates.Project),
                            typeof(Storage.Model.PriceRules.Aggregates.Project),
                            typeof(Storage.Model.ProjectRules.Aggregates.Project),
                        }
                    },
                    { typeof(Position), new []
                        {
                            typeof(Storage.Model.AdvertisementRules.Aggregates.Position),
                            typeof(Storage.Model.PriceRules.Aggregates.Position),
                            typeof(Storage.Model.ProjectRules.Aggregates.Position),
                        }
                    },
                    { typeof(Firm), new []
                        {
                            typeof(Storage.Model.AdvertisementRules.Aggregates.Firm),
                            typeof(Storage.Model.FirmRules.Aggregates.Firm)
                        }
                    },
                    { typeof(Theme), new []
                        {
                            typeof(Storage.Model.ThemeRules.Aggregates.Theme),
                            typeof(Storage.Model.PriceRules.Aggregates.Theme)
                        }
                    },
                    { typeof(Price), new []
                        {
                            typeof(Storage.Model.PriceRules.Aggregates.Price)
                        }
                    },
                    { typeof(Account), new []
                        {
                            typeof(Storage.Model.AccountRules.Aggregates.Account)
                        }
                    },
                    { typeof(Advertisement), new []
                        {
                            typeof(Storage.Model.AdvertisementRules.Aggregates.Advertisement)
                        }
                    },
                    { typeof(AdvertisementElementTemplate), new []
                        {
                            typeof(Storage.Model.AdvertisementRules.Aggregates.AdvertisementElementTemplate)
                        }
                    },
                    { typeof(FirmAddress), new []
                        {
                            typeof(Storage.Model.ProjectRules.Aggregates.FirmAddress)
                        }
                    },
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
                return MapDataObjectToAggregate(createdEvent.DataObjectType).Select(x => new InitializeAggregateCommand(x, createdEvent.DataObjectId)).ToList();
            }

            var updatedEvent = @event as DataObjectUpdatedEvent;
            if (updatedEvent != null)
            {
                return MapDataObjectToAggregate(updatedEvent.DataObjectType).Select(x => new RecalculateAggregateCommand(x, updatedEvent.DataObjectId)).ToList();
            }

            var deletedEvent = @event as DataObjectDeletedEvent;
            if (deletedEvent != null)
            {
                return MapDataObjectToAggregate(deletedEvent.DataObjectType).Select(x => new DestroyAggregateCommand(x, deletedEvent.DataObjectId)).ToList();
            }

            // TODO: нормально обрабатывать events
            var outdatedEvent = @event as RelatedDataObjectOutdatedEvent<long>;
            if (outdatedEvent != null)
            {
                return MapDataObjectToAggregate(outdatedEvent.RelatedDataObjectType).Select(x => new RecalculateAggregateCommand(x, outdatedEvent.RelatedDataObjectId)).ToList();
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

        private IReadOnlyCollection<Type> MapDataObjectToAggregate(Type dataObjectType)
        {
            IReadOnlyCollection<Type> result;
            if (!AggregateRoots.TryGetValue(dataObjectType, out result))
            {
                throw new ArgumentException($"Type {dataObjectType.FullName} is not mapped with aggregate root type", nameof(dataObjectType));
            }

            return result;
        }
    }
}