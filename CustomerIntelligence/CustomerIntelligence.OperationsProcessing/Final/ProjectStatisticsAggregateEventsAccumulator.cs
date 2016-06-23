using System;
using System.Collections.Generic;

using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.CustomerIntelligence.Replication;
using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Final
{
    public class ProjectStatisticsAggregateEventsAccumulator :
        MessageProcessingContextAccumulatorBase<StatisticsEventsFlow, EventMessage, AggregatableMessage<ICommand>>
    {
        private static readonly IReadOnlyDictionary<Type, Type> AggregateRootTypes
            = new Dictionary<Type, Type>
                {
                    { typeof(Storage.Model.Bit.FirmCategoryForecast), typeof(Storage.Model.Statistics.ProjectStatistics) },
                    { typeof(Storage.Model.Bit.FirmCategoryStatistics), typeof(Storage.Model.Statistics.ProjectStatistics) },
                    { typeof(Storage.Model.Bit.FirmForecast), typeof(Storage.Model.Statistics.ProjectStatistics) },
                    { typeof(Storage.Model.Bit.ProjectCategoryStatistics), typeof(Storage.Model.Statistics.ProjectStatistics) }
                };

        protected override AggregatableMessage<ICommand> Process(EventMessage message)
            => new AggregatableMessage<ICommand>
                {
                    TargetFlow = MessageFlow,
                    Commands = CreateCommands(message.Event),
                };

        private static IReadOnlyCollection<ICommand> CreateCommands(IEvent @event)
        {
            var replacedEvent = @event as DataObjectReplacedEvent;
            if (replacedEvent != null)
            {
                return new[] { new RecalculateAggregateCommand(AggregateRootTypes[replacedEvent.DataObjectType], replacedEvent.DataObjectId) };
            }

            var outdatedEvent = @event as RelatedDataObjectOutdatedEvent<StatisticsKey>;
            if (outdatedEvent != null)
            {
                return new[]
                    {
                        new RecalculateEntityCommand(
                            AggregateRootTypes[outdatedEvent.RelatedDataObjectType],
                            outdatedEvent.RelatedDataObjectId.ProjectId,
                            outdatedEvent.RelatedDataObjectType,
                            outdatedEvent.RelatedDataObjectId.CategoryId)
                    };
            }

            var batchProcessedEvent = @event as BatchProcessedEvent;
            if (batchProcessedEvent != null)
            {
                return new[] { new RecordDelayCommand(batchProcessedEvent.EventTime) };
            }

            throw new ArgumentException($"Unexpected event '{@event}'", nameof(@event));
        }
    }
}