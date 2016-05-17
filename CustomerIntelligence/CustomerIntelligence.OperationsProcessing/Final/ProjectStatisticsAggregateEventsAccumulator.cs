using System;
using System.Collections.Generic;

using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.CustomerIntelligence.Replication;
using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Commands;
using NuClear.Replication.OperationsProcessing;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Final
{
    public class ProjectStatisticsAggregateEventsAccumulator :
        MessageProcessingContextAccumulatorBase<StatisticsEventsFlow, EventMessage, AggregatableMessage<IAggregateCommand>>
    {
        private static readonly IReadOnlyDictionary<Type, Type> AggregateRootTypes
            = new Dictionary<Type, Type>
                {
                    { typeof(Storage.Model.Bit.FirmCategoryForecast), typeof(Storage.Model.Statistics.ProjectStatistics) },
                    { typeof(Storage.Model.Bit.FirmCategoryStatistics), typeof(Storage.Model.Statistics.ProjectStatistics) },
                    { typeof(Storage.Model.Bit.FirmForecast), typeof(Storage.Model.Statistics.ProjectStatistics) },
                    { typeof(Storage.Model.Bit.ProjectCategoryStatistics), typeof(Storage.Model.Statistics.ProjectStatistics) }
                };

        protected override AggregatableMessage<IAggregateCommand> Process(EventMessage message)
            => new AggregatableMessage<IAggregateCommand>
                {
                    TargetFlow = MessageFlow,
                    Commands = CreateCommands(message.Event),
                    EventHappenedTime = message.Event.Time,
                };

        private static IReadOnlyCollection<IAggregateCommand> CreateCommands(IEvent @event)
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

            throw new ArgumentException($"Unexpected event '{@event}'", nameof(@event));
        }
    }
}