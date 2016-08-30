using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.OperationsLogging.API;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Commands;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Telemetry;
using NuClear.Telemetry;
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.OperationsProcessing.Final
{
    public sealed class AggregateCommandsHandler : IMessageProcessingHandler
    {
        private readonly IAggregateActorFactory _aggregateActorFactory;
        private readonly ITelemetryPublisher _telemetryPublisher;
        private readonly IEventLogger _eventLogger;
        private readonly ITracer _tracer;

        public AggregateCommandsHandler(IAggregateActorFactory aggregateActorFactory, ITelemetryPublisher telemetryPublisher, ITracer tracer, IEventLogger eventLogger)
        {
            _aggregateActorFactory = aggregateActorFactory;
            _telemetryPublisher = telemetryPublisher;
            _tracer = tracer;
            _eventLogger = eventLogger;
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            try
            {
                var messages = processingResultsMap.SelectMany(pair => pair.Value)
                                                   .Cast<AggregatableMessage<ICommand>>()
                                                   .ToArray();

                Handle(messages.SelectMany(x => x.Commands).OfType<IAggregateCommand>().ToArray());
                Handle(messages.SelectMany(message => message.Commands.OfType<IncrementStateCommand>()).ToArray());
                Handle(messages.SelectMany(x => x.Commands).OfType<RecordDelayCommand>().ToArray());

                return processingResultsMap.Keys.Select(bucketId => MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded());
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error when calculating aggregates");
                return processingResultsMap.Keys.Select(bucketId => MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex));
            }
        }

        private void Handle(IReadOnlyCollection<RecordDelayCommand> commands)
        {
            if (!commands.Any())
            {
                return;
            }

            var eldestEventTime = commands.Min(x => x.EventTime);
            var delta = DateTime.UtcNow - eldestEventTime;
            _eventLogger.Log(new IEvent[] { new AggregatesBatchProcessedEvent(DateTime.UtcNow) });
            _telemetryPublisher.Publish<AggregateProcessingDelayIdentity>((long)delta.TotalMilliseconds);
        }

        private void Handle(IReadOnlyCollection<IncrementStateCommand> commands)
        {
            if (!commands.Any())
            {
                return;
            }

            var states = commands.SelectMany(command => command.States).ToArray();
            _eventLogger.Log(new IEvent[] { new AggregatesStateIncrementedEvent(states) });
        }

        private void Handle(IReadOnlyCollection<IAggregateCommand> commands)
        {
            var aggregateRootTypes = commands.Select(x => x.AggregateRootType).ToArray();
            var actors = _aggregateActorFactory.Create(aggregateRootTypes);

            // TODO: Can agreggate actors be executed in parallel? See https://github.com/2gis/nuclear-river/issues/76
            foreach (var actor in actors)
            {
                var actorName = actor.GetType().GetFriendlyName();

                using (var transaction = new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
                {
                    using (Probe.Create($"ETL2 {actorName}"))
                    {
                        actor.ExecuteCommands(commands);
                    }

                    transaction.Complete();
                }
            }

            _telemetryPublisher.Publish<AggregateProcessedOperationCountIdentity>(commands.Count);
        }
    }
}