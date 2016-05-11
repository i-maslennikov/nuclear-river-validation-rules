using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.CustomerIntelligence.Replication;
using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.Core.Commands;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Final
{
    public class ProjectStatisticsAggregateEventsAccumulator :
        MessageProcessingContextAccumulatorBase<StatisticsEventsFlow, PerformedOperationsFinalProcessingMessage, AggregatableMessage<IAggregateCommand>>
    {
        private readonly IEventSerializer _serializer;

        private static readonly IReadOnlyDictionary<Type, Type> AggregateRootTypes
            = new Dictionary<Type, Type>
                {
                    { typeof(Storage.Model.Bit.FirmCategoryForecast), typeof(Storage.Model.Statistics.ProjectStatistics) },
                    { typeof(Storage.Model.Bit.FirmCategoryStatistics), typeof(Storage.Model.Statistics.ProjectStatistics) },
                    { typeof(Storage.Model.Bit.FirmForecast), typeof(Storage.Model.Statistics.ProjectStatistics) },
                    { typeof(Storage.Model.Bit.ProjectCategoryStatistics), typeof(Storage.Model.Statistics.ProjectStatistics) }
                };

        public ProjectStatisticsAggregateEventsAccumulator(IEventSerializer serializer)
        {
            _serializer = serializer;
        }

        protected override AggregatableMessage<IAggregateCommand> Process(PerformedOperationsFinalProcessingMessage message)
        {
            var events = message.FinalProcessings.Select(x => _serializer.Deserialize(x)).ToArray();
            var commands = new List<IAggregateCommand>();
            foreach(var @event in events)
            {
                var replacedEvent = @event as DataObjectReplacedEvent;
                if (replacedEvent != null)
                {
                    commands.Add(new RecalculateAggregateCommand(AggregateRootTypes[replacedEvent.DataObjectType], replacedEvent.DataObjectId));
                    continue;
                }

                var outdatedEvent = @event as RelatedDataObjectOutdatedEvent<StatisticsKey>;
                if (outdatedEvent != null)
                {
                    commands.Add(new RecalculateEntityCommand(
                                     AggregateRootTypes[outdatedEvent.RelatedDataObjectType],
                                     outdatedEvent.RelatedDataObjectId.ProjectId,
                                     outdatedEvent.RelatedDataObjectType,
                                     outdatedEvent.RelatedDataObjectId.CategoryId));
                    continue;
                }
            }

            return new AggregatableMessage<IAggregateCommand>
                {
                    TargetFlow = MessageFlow,
                    Commands = commands,
                    EventHappenedTime = message.FinalProcessings.Min(x => x.CreatedOn),
                };
        }
    }
}