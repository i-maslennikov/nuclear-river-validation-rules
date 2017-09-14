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
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.OperationsProcessing.AggregatesFlow
{
    public sealed class AggregatesFlowHandler : IMessageProcessingHandler
    {
        private readonly IAggregateActorFactory _aggregateActorFactory;
        private readonly AggregatesFlowTelemetryPublisher _telemetryPublisher;
        private readonly IEventLogger _eventLogger;
        private readonly ITracer _tracer;
        private readonly TransactionOptions _transactionOptions;

        public AggregatesFlowHandler(IAggregateActorFactory aggregateActorFactory, AggregatesFlowTelemetryPublisher telemetryPublisher, ITracer tracer, IEventLogger eventLogger)
        {
            _aggregateActorFactory = aggregateActorFactory;
            _telemetryPublisher = telemetryPublisher;
            _tracer = tracer;
            _eventLogger = eventLogger;
            _transactionOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero };
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            try
            {
                using (Probe.Create("ETL2 Transforming"))
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, _transactionOptions))
                {
                    var commands = processingResultsMap.SelectMany(x => x.Value).Cast<AggregatableMessage<ICommand>>().SelectMany(x => x.Commands).ToList();

                    var syncEvents = Handle(commands.OfType<IAggregateCommand>().ToList())
                                    .Select(x => new FlowEvent(AggregatesFlow.Instance, x)).ToList();
                    var stateEvents = Handle(commands.OfType<IncrementErmStateCommand>().ToList()).Concat(
                                      Handle(commands.OfType<IncrementAmsStateCommand>().ToList())).Concat(
                                      Handle(commands.OfType<LogDelayCommand>().ToList()))
                                      .Select(x => new FlowEvent(AggregatesFlow.Instance, x));

                    using (new TransactionScope(TransactionScopeOption.Suppress))
                        _eventLogger.Log<IEvent>(syncEvents);

                    transaction.Complete();

                    using (new TransactionScope(TransactionScopeOption.Suppress))
                        _eventLogger.Log<IEvent>(syncEvents.Concat(stateEvents).ToList());
                }

                return processingResultsMap.Keys.Select(bucketId => MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded());
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error when calculating aggregates");
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

        private static IEnumerable<IEvent> Handle(IReadOnlyCollection<IncrementAmsStateCommand> commands)
        {
            if (!commands.Any())
            {
                return Array.Empty<IEvent>();
            }

            var maxAmsState = commands.Select(x => x.State).OrderByDescending(x => x.Offset).First();
            return new IEvent[] { new AmsStateIncrementedEvent(maxAmsState) };
        }

        private static IEnumerable<IEvent> Handle(IReadOnlyCollection<IncrementErmStateCommand> commands)
        {
            if (!commands.Any())
            {
                return Array.Empty<IEvent>();
            }

            return new IEvent[] { new ErmStateIncrementedEvent(commands.SelectMany(x => x.States)) };
        }

        private IEnumerable<IEvent> Handle(IReadOnlyCollection<IAggregateCommand> commands)
        {
            if (!commands.Any())
            {
                return Array.Empty<IEvent>();
            }

            var actors = _aggregateActorFactory.Create(new HashSet<Type>(commands.Select(x => x.AggregateRootType)));
            var events = new HashSet<IEvent>();

            foreach (var actor in actors)
            {
                events.UnionWith(actor.ExecuteCommands(commands));
            }

            return events;
        }
    }
}