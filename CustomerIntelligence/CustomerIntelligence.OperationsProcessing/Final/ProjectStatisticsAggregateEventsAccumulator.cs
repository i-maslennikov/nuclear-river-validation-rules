using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Events;
using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Final
{
    public class ProjectStatisticsAggregateEventsAccumulator :
        MessageProcessingContextAccumulatorBase<StatisticsEventsFlow, PerformedOperationsFinalProcessingMessage, OperationAggregatableMessage<IAggregateCommand>>
    {
        private readonly IEventSerializer _serializer;

        private static readonly IReadOnlyDictionary<Type, Type> AggregateRootTypes
            = new Dictionary<Type, Type>
                {
                    { typeof(Domain.Model.Bit.FirmCategoryForecast), typeof(Domain.Model.Statistics.ProjectStatistics) },
                    { typeof(Domain.Model.Bit.FirmCategoryStatistics), typeof(Domain.Model.Statistics.ProjectStatistics) },
                    { typeof(Domain.Model.Bit.FirmForecast), typeof(Domain.Model.Statistics.ProjectStatistics) },
                    { typeof(Domain.Model.Bit.ProjectCategoryStatistics), typeof(Domain.Model.Statistics.ProjectStatistics) }
                };

        public ProjectStatisticsAggregateEventsAccumulator(IEventSerializer serializer)
        {
            _serializer = serializer;
        }

        protected override OperationAggregatableMessage<IAggregateCommand> Process(PerformedOperationsFinalProcessingMessage message)
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

            return new OperationAggregatableMessage<IAggregateCommand>
                {
                    TargetFlow = MessageFlow,
                    Commands = commands,
                    OperationTime = message.FinalProcessings.Min(x => x.CreatedOn),
                };
        }
    }
}