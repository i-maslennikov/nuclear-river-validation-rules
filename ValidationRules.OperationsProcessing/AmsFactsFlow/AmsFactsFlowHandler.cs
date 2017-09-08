﻿using System;
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
using NuClear.Tracing.API;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.OperationsProcessing.AmsFactsFlow
{
    public sealed class AmsFactsFlowHandler : IMessageProcessingHandler
    {
        private readonly IDataObjectsActorFactory _dataObjectsActorFactory;
        private readonly SyncEntityNameActor _syncEntityNameActor;
        private readonly IEventLogger _eventLogger;
        private readonly ITracer _tracer;
        private readonly AmsFactsFlowTelemetryPublisher _telemetryPublisher;
        private readonly TransactionOptions _transactionOptions;

        public AmsFactsFlowHandler(
            IDataObjectsActorFactory dataObjectsActorFactory,
            SyncEntityNameActor syncEntityNameActor,
            IEventLogger eventLogger,
            AmsFactsFlowTelemetryPublisher telemetryPublisher,
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
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, _transactionOptions))
                {
                    var lookups = processingResultsMap.SelectMany(x => x.Value).Cast<AggregatableMessage<ICommand>>().ToLookup(x => x.TargetFlow, x => x.Commands);
                    foreach (var lookup in lookups)
                    {
                        var flow = lookup.Key;
                        var commands = lookup.SelectMany(x => x).ToList();

                        var replaceEvents = Handle(commands.OfType<IReplaceDataObjectCommand>().ToList()).Select(x => new FlowEvent(flow, x)).ToList();
                        var stateEvents = Handle(commands.OfType<IncrementAmsStateCommand>().ToList()).Select(x => new FlowEvent(flow, x));

                        using (new TransactionScope(TransactionScopeOption.Suppress))
                            _eventLogger.Log<IEvent>(replaceEvents);

                        transaction.Complete();

                        using (new TransactionScope(TransactionScopeOption.Suppress))
                            _eventLogger.Log<IEvent>(replaceEvents.Concat(stateEvents).ToList());
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

        private IEnumerable<IEvent> Handle(IReadOnlyCollection<IncrementAmsStateCommand> commands)
        {
            if (!commands.Any())
            {
                return Array.Empty<IEvent>();
            }

            var eldestEventTime = commands.Min(x => x.State.UtcDateTime);
            var delta = DateTime.UtcNow - eldestEventTime;
            _telemetryPublisher.Delay((int)delta.TotalMilliseconds);

            var maxAmsState = commands.Select(x => x.State).OrderByDescending(x => x.Offset).First();
            return new IEvent[]
            {
                new AmsStateIncrementedEvent(maxAmsState),
                new DelayLoggedEvent(DateTime.UtcNow)
            };
        }

        private IEnumerable<IEvent> Handle(IReadOnlyCollection<IReplaceDataObjectCommand> commands)
        {
            if (!commands.Any())
            {
                return Array.Empty<IEvent>();
            }

            var actors = _dataObjectsActorFactory.Create();
            var events = new HashSet<IEvent>();

            foreach (var actor in actors)
            {
                events.UnionWith(actor.ExecuteCommands(commands));
            }

            _syncEntityNameActor.ExecuteCommands(commands);

            return events;
        }
    }
}