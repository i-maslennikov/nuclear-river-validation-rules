using System;
using System.Collections.Generic;
using System.Linq;

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

namespace NuClear.ValidationRules.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromErmHandler : IMessageProcessingHandler
    {
        private readonly IDataObjectsActorFactory _dataObjectsActorFactory;
        private readonly IEventLogger _eventLogger;
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public ImportFactsFromErmHandler(
            IDataObjectsActorFactory dataObjectsActorFactory,
            IEventLogger eventLogger,
            ITelemetryPublisher telemetryPublisher,
            ITracer tracer)
        {
            _dataObjectsActorFactory = dataObjectsActorFactory;
            _eventLogger = eventLogger;
            _telemetryPublisher = telemetryPublisher;
            _tracer = tracer;
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            try
            {
                using (Probe.Create("ETL1 Transforming"))
                {
                    var commands = processingResultsMap.SelectMany(x => x.Value).Cast<AggregatableMessage<ICommand>>().SelectMany(x => x.Commands).ToList();

                    Handle(commands.OfType<ISyncDataObjectCommand>().ToList());
                    Handle(commands.OfType<IncrementStateCommand>().ToList());
                    Handle(commands.OfType<RecordDelayCommand>().ToList());

                    return processingResultsMap.Keys.Select(bucketId => MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded());
                }
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error when import facts for ERM");
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
            _eventLogger.Log(new IEvent[] { new FactsBatchProcessedEvent(DateTime.UtcNow) });
            _telemetryPublisher.Publish<PrimaryProcessingDelayIdentity>((long)delta.TotalMilliseconds);
        }

        private void Handle(IReadOnlyCollection<IncrementStateCommand> commands)
        {
            if (!commands.Any())
            {
                return;
            }

            var states = commands.SelectMany(command => command.States).ToArray();
            _eventLogger.Log(new IEvent[] { new FactsStateIncrementedEvent(states) });
        }

        private void Handle(IReadOnlyCollection<ISyncDataObjectCommand> commands)
        {
            if (!commands.Any())
            {
                return;
            }

            // TODO: Can actors be executed in parallel? See https://github.com/2gis/nuclear-river/issues/76
            var actors = _dataObjectsActorFactory.Create();
            foreach (var actor in actors)
            {
                IReadOnlyCollection<IEvent> events;

                var actorType = actor.GetType().GetFriendlyName();
                using (Probe.Create($"ETL1 {actorType}"))
                {
                    events = new HashSet<IEvent>(actor.ExecuteCommands(commands));
                }

                if (events.Any())
                {
                    _eventLogger.Log(events);
                }
            }

            _telemetryPublisher.Publish<ErmProcessedOperationCountIdentity>(commands.Count);
        }
    }
}
