using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.OperationsLogging.API;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Commands;
using NuClear.Replication.Core.Settings;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Telemetry;
using NuClear.Telemetry;
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromErmHandler : IMessageProcessingHandler
    {
        private readonly IReplicationSettings _replicationSettings;
        private readonly IDataObjectsActorFactory _dataObjectsActorFactory;
        private readonly IEventLogger _eventLogger;
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public ImportFactsFromErmHandler(
            IReplicationSettings replicationSettings,
            IDataObjectsActorFactory dataObjectsActorFactory,
            IEventLogger eventLogger,
            ITelemetryPublisher telemetryPublisher,
            ITracer tracer)
        {
            _replicationSettings = replicationSettings;
            _dataObjectsActorFactory = dataObjectsActorFactory;
            _eventLogger = eventLogger;
            _telemetryPublisher = telemetryPublisher;
            _tracer = tracer;
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            try
            {
                var messages = processingResultsMap.SelectMany(pair => pair.Value)
                                                   .Cast<AggregatableMessage<ICommand>>()
                                                   .ToArray();

                Handle(processingResultsMap.Keys.ToArray(), messages.SelectMany(message => message.Commands.OfType<ISyncDataObjectCommand>()).ToArray());
                Handle(messages.SelectMany(message => message.Commands.OfType<RecordDelayCommand>()).ToArray());

                return processingResultsMap.Keys.Select(bucketId => MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded());
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
            _eventLogger.Log(new IEvent[] { new BatchProcessedEvent(DateTime.UtcNow) });
            _telemetryPublisher.Publish<PrimaryProcessingDelayIdentity>((long)delta.TotalMilliseconds);
        }

        private void Handle(IReadOnlyCollection<Guid> bucketIds, IReadOnlyCollection<ISyncDataObjectCommand> commands)
        {
            _tracer.Debug("Executing fact commands started");

            var events = new List<IEvent>();
            using (Probe.Create("ETL1 Transforming"))
            {
                // TODO: Can actors be executed in parallel? See https://github.com/2gis/nuclear-river/issues/76
                var actors = _dataObjectsActorFactory.Create();
                foreach (var actor in actors)
                {
                    var actorType = actor.GetType().GetFriendlyName();
                    using (Probe.Create("ETL1 Transforming", actorType))
                    {
                        _tracer.Debug($"Applying changes to target facts storage with actor {actorType}");
                        foreach(var batch in commands.CreateBatches(_replicationSettings.ReplicationBatchSize))
                        {
                            events.AddRange(actor.ExecuteCommands(batch));
                        }
                    }
                }
            }

            if (events.Count > 1000 * bucketIds.Count)
            {
                _tracer.Warn($"Messages produced huge events amount: from {bucketIds.Count} TUCs to {events.Count} commands\n" +
                             string.Join(", ", bucketIds));
            }

            _telemetryPublisher.Publish<ErmProcessedOperationCountIdentity>(commands.Count);

            _eventLogger.Log(events);

            _tracer.Debug("Executing fact commands finished");
        }
    }
}