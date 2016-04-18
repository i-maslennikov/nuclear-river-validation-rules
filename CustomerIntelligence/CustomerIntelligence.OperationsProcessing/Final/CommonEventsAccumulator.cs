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

namespace NuClear.CustomerIntelligence.OperationsProcessing.Final
{
    public sealed class CommonEventsAccumulator :
        MessageProcessingContextAccumulatorBase<CommonEventsFlow, PerformedOperationsFinalProcessingMessage, OperationAggregatableMessage<IAggregateCommand>>
    {
        private static readonly IReadOnlyDictionary<Type, Type> AggregateRootTypes
            = new Dictionary<Type, Type>
                {
                    { typeof(Domain.Model.Facts.Firm), typeof(Domain.Model.CI.Firm) }
                };

        private readonly IEventSerializer _serializer;

        public CommonEventsAccumulator(IEventSerializer serializer)
        {
            _serializer = serializer;
        }

        protected override OperationAggregatableMessage<IAggregateCommand> Process(PerformedOperationsFinalProcessingMessage message)
        {
            var events = message.FinalProcessings.Select(x => _serializer.Deserialize(x)).ToArray();
            var commands = new List<IAggregateCommand>();
            foreach (var @event in events)
            {
                var createdEvent = @event as DataObjectCreatedEvent;
                if (createdEvent != null)
                {
                    commands.Add(new InitializeAggregateCommand(AggregateRootTypes[createdEvent.DataObjectType], createdEvent.DataObjectId));
                    continue;
                }

                var updatedEvent = @event as DataObjectUpdatedEvent;
                if (updatedEvent != null)
                {
                    commands.Add(new RecalculateAggregateCommand(AggregateRootTypes[updatedEvent.DataObjectType], updatedEvent.DataObjectId));
                    continue;
                }

                var deletedEvent = @event as DataObjectDeletedEvent;
                if (deletedEvent != null)
                {
                    commands.Add(new DestroyAggregateCommand(AggregateRootTypes[deletedEvent.DataObjectType], deletedEvent.DataObjectId));
                    continue;
                }

                var outdatedEvent = @event as RelatedDataObjectOutdatedEvent<long>;
                if (outdatedEvent != null)
                {
                    commands.Add(new RecalculateAggregateCommand(AggregateRootTypes[outdatedEvent.RelatedDataObjectType], outdatedEvent.RelatedDataObjectId));
                    continue;
                }
            }

            return new OperationAggregatableMessage<IAggregateCommand>
                {
                    TargetFlow = MessageFlow,
                    Commands = commands,
                    OperationTime = message.FinalProcessings.Min(x => x.CreatedOn)
                };
        }
    }
}