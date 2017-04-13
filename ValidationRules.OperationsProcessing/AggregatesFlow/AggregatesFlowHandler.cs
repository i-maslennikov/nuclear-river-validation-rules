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

namespace NuClear.ValidationRules.OperationsProcessing.AggregatesFlow
{
    public sealed class AggregatesFlowHandler : IMessageProcessingHandler
    {
        private readonly IAggregateActorFactory _aggregateActorFactory;
        private readonly ITelemetryPublisher _telemetryPublisher;
        private readonly IEventLogger _eventLogger;
        private readonly ITracer _tracer;

        public AggregatesFlowHandler(IAggregateActorFactory aggregateActorFactory, ITelemetryPublisher telemetryPublisher, ITracer tracer, IEventLogger eventLogger)
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
                using (Probe.Create("ETL2 Transforming"))
                {
                    var commands = processingResultsMap.SelectMany(x => x.Value).Cast<AggregatableMessage<ICommand>>().SelectMany(x => x.Commands).ToList();

                    Handle(commands.OfType<IAggregateCommand>().ToList());
                    Handle(commands.OfType<IncrementStateCommand>().ToList());
                    Handle(commands.OfType<LogDelayCommand>().ToList());

                    return processingResultsMap.Keys.Select(bucketId => MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded());
                }
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error when calculating aggregates");
                return processingResultsMap.Keys.Select(bucketId => MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex));
            }
        }

        private void Handle(IReadOnlyCollection<LogDelayCommand> commands)
        {
            if (!commands.Any())
            {
                return;
            }

            var eldestEventTime = commands.Min(x => x.EventTime);
            var delta = DateTime.UtcNow - eldestEventTime;
            _eventLogger.Log(new IEvent[] { new AggregatesDelayLoggedEvent(DateTime.UtcNow) });
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
            if (!commands.Any())
            {
                return;
            }

            var actors = _aggregateActorFactory.Create(new HashSet<Type>(commands.Select(x => x.AggregateRootType)));

            var events = new HashSet<IEvent>();

            // TODO: Can agreggate actors be executed in parallel? See https://github.com/2gis/nuclear-river/issues/76
            foreach (var actor in actors)
            {
                var transactionOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero };
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                {
                    events.UnionWith(actor.ExecuteCommands(commands));
                    transaction.Complete();
                }
            }

            if (events.Any())
            {
                _eventLogger.Log(events);
            }

            _telemetryPublisher.Publish<AggregateProcessedOperationCountIdentity>(commands.Count);
        }
    }
}