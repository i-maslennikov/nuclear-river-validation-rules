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
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.OperationsProcessing.FactsFlow
{
    public sealed class FactsFlowHandler : IMessageProcessingHandler
    {
        private readonly IDataObjectsActorFactory _dataObjectsActorFactory;
        private readonly SyncEntityNameActor _syncEntityNameActor;
        private readonly IEventLogger _eventLogger;
        private readonly ITracer _tracer;
        private readonly FactsFlowTelemetryPublisher _telemetryPublisher;
        private readonly TransactionOptions _transactionOptions;

        public FactsFlowHandler(
            IDataObjectsActorFactory dataObjectsActorFactory,
            SyncEntityNameActor syncEntityNameActor,
            IEventLogger eventLogger,
            FactsFlowTelemetryPublisher telemetryPublisher,
            ITracer tracer)
        {
            _dataObjectsActorFactory = dataObjectsActorFactory;
            _syncEntityNameActor = syncEntityNameActor;
            _eventLogger = eventLogger;
            _telemetryPublisher = telemetryPublisher;
            _tracer = tracer;
            _transactionOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero };
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            try
            {
                using (Probe.Create("ETL1 Transforming"))
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, _transactionOptions))
                {
                    var lookups = processingResultsMap.SelectMany(x => x.Value).Cast<AggregatableMessage<ICommand>>().ToLookup(x => x.TargetFlow, x => x.Commands);
                    foreach (var lookup in lookups)
                    {
                        var flow = lookup.Key;
                        var commands = lookup.SelectMany(x => x).ToList();

                        var syncEvents = Handle(commands.OfType<ISyncDataObjectCommand>().ToList()).Select(x => new FlowEvent(flow, x)).ToList();
                        var stateEvents = Handle(commands.OfType<IncrementErmStateCommand>().ToList()).Concat(
                                          Handle(commands.OfType<LogDelayCommand>().ToList()))
                                          .Select(x => new FlowEvent(flow, x));

                        using (new TransactionScope(TransactionScopeOption.Suppress))
                            _eventLogger.Log<IEvent>(syncEvents);

                        transaction.Complete();

                        using (new TransactionScope(TransactionScopeOption.Suppress))
                            _eventLogger.Log<IEvent>(syncEvents.Concat(stateEvents).ToList());
                    }
                }

                return processingResultsMap.Keys.Select(bucketId => MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded());
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error when import facts for ERM");
                return processingResultsMap.Keys.Select(bucketId => MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex));
            }
        }

        private IEnumerable<IEvent> Handle(IReadOnlyCollection<LogDelayCommand> commands)
        {
            if (!commands.Any())
            {
                return Array.Empty<IEvent>();
            }

            var eldestEventTime = commands.Min(x => x.EventTime);
            var delta = DateTime.UtcNow - eldestEventTime;
            _telemetryPublisher.Delay((int)delta.TotalMilliseconds);
            return new IEvent[] { new DelayLoggedEvent(DateTime.UtcNow) };
        }

        private static IEnumerable<IEvent> Handle(IReadOnlyCollection<IncrementErmStateCommand> commands)
        {
            if (!commands.Any())
            {
                return Array.Empty<IEvent>();
            }

            return new IEvent[] { new ErmStateIncrementedEvent(commands.SelectMany(x => x.States)) };
        }

        private IEnumerable<IEvent> Handle(IReadOnlyCollection<ISyncDataObjectCommand> commands)
        {
            if (!commands.Any())
            {
                return Array.Empty<IEvent>();
            }

            var actors = _dataObjectsActorFactory.Create();
            var events = new HashSet<IEvent>();

            foreach (var actor in actors)
            {
                var actorType = actor.GetType().GetFriendlyName();
                using (Probe.Create($"ETL1 {actorType}"))
                {
                    events.UnionWith(actor.ExecuteCommands(commands));
                }
            }

            _syncEntityNameActor.ExecuteCommands(commands);

            return events;
        }
    }
}
