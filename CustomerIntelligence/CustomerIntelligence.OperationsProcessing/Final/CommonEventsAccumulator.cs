using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.Core.Commands;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Final
{
    public sealed class CommonEventsAccumulator :
        MessageProcessingContextAccumulatorBase<CommonEventsFlow, PerformedOperationsFinalProcessingMessage, AggregatableMessage<IAggregateCommand>>
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

        private readonly IEventSerializer _serializer;

        public CommonEventsAccumulator(IEventSerializer serializer)
        {
            _serializer = serializer;
        }

        protected override AggregatableMessage<IAggregateCommand> Process(PerformedOperationsFinalProcessingMessage message)
        {
            var events = message.FinalProcessings.Select(x => _serializer.Deserialize(x)).ToArray();
            var commands = new List<IAggregateCommand>();
            foreach (var @event in events)
            {
                var createdEvent = @event as DataObjectCreatedEvent;
                if (createdEvent != null)
                {
                    commands.Add(new InitializeAggregateCommand(AggregateRoots[createdEvent.DataObjectType], createdEvent.DataObjectId));
                    continue;
                }

                var updatedEvent = @event as DataObjectUpdatedEvent;
                if (updatedEvent != null)
                {
                    commands.Add(new RecalculateAggregateCommand(AggregateRoots[updatedEvent.DataObjectType], updatedEvent.DataObjectId));
                    continue;
                }

                var deletedEvent = @event as DataObjectDeletedEvent;
                if (deletedEvent != null)
                {
                    commands.Add(new DestroyAggregateCommand(AggregateRoots[deletedEvent.DataObjectType], deletedEvent.DataObjectId));
                    continue;
                }

                var outdatedEvent = @event as RelatedDataObjectOutdatedEvent<long>;
                if (outdatedEvent != null)
                {
                    commands.Add(new RecalculateAggregateCommand(AggregateRoots[outdatedEvent.RelatedDataObjectType], outdatedEvent.RelatedDataObjectId));
                    continue;
                }
            }

            return new AggregatableMessage<IAggregateCommand>
                {
                    TargetFlow = MessageFlow,
                    Commands = commands,
                    EventHappenedTime = message.FinalProcessings.Min(x => x.CreatedOn)
                };
        }
    }
}