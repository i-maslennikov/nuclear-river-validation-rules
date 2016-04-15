using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Model;
using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Settings;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Identities.Telemetry;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.River.Common.Metadata;
using NuClear.Telemetry;
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromErmHandler : IMessageProcessingHandler
    {
        private readonly IReplicationSettings _replicationSettings;
        private readonly IDataObjectsActorFactory _dataObjectsActorFactory;
        private readonly IEventSender _eventSender;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public ImportFactsFromErmHandler(
            IReplicationSettings replicationSettings,
            IDataObjectsActorFactory dataObjectsActorFactory,
            IEventSender eventSender,
            ITelemetryPublisher telemetryPublisher,
            IEventDispatcher eventDispatcher,
            ITracer tracer)
        {
            _replicationSettings = replicationSettings;
            _dataObjectsActorFactory = dataObjectsActorFactory;
            _eventSender = eventSender;
            _telemetryPublisher = telemetryPublisher;
            _eventDispatcher = eventDispatcher;
            _tracer = tracer;
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            try
            {
                var messages = processingResultsMap.SelectMany(pair => pair.Value)
                                                   .Cast<OperationAggregatableMessage<SyncDataObjectCommand>>()
                                                   .ToArray();

                Handle(messages.SelectMany(message => message.Commands).ToArray());

                var eldestOperationPerformTime = messages.Min(message => message.OperationTime);
                _telemetryPublisher.Publish<PrimaryProcessingDelayIdentity>((long)(DateTime.UtcNow - eldestOperationPerformTime).TotalMilliseconds);

                return processingResultsMap.Keys.Select(bucketId => MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded());
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error when import facts for ERM");
                return processingResultsMap.Keys.Select(bucketId => MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex));
            }
        }

        private void Handle(IReadOnlyCollection<SyncDataObjectCommand> commands)
        {
            _tracer.Debug("Executing fact commands started");

            var events = Enumerable.Empty<IEvent>();
            using (Probe.Create("ETL1 Transforming"))
            {
                var slices = commands.GroupBy(x => x.DataObjectType)
                                       .OrderByDescending(slice => slice.Key, new CustomerIntelligenceFactTypePriorityComparer());
                foreach (var slice in slices)
                {
                    var dataObjectType = slice.Key;
                    using (Probe.Create("ETL1 Transforming", dataObjectType.Name))
                    {
                        var actor = _dataObjectsActorFactory.Create(dataObjectType);
                        foreach (var batch in slice.Distinct().CreateBatches(_replicationSettings.ReplicationBatchSize))
                        {
                            _tracer.Debug("Apply changes to target facts storage");
                            events = events.Concat(actor.ExecuteCommands(batch));
                        }
                    }
                }
            }

            _telemetryPublisher.Publish<ErmProcessedOperationCountIdentity>(commands.Count);

            // We always need to use different transaction scope to operate with operation sender because it has its own store
            using (var pushTransaction = new TransactionScope(TransactionScopeOption.RequiresNew,
                                                              new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
            {
                _tracer.Debug("Pushing messages");
                DispatchEvents(events.Distinct().ToArray());
                pushTransaction.Complete();
            }

            _tracer.Debug("Executing fact commands finished");
        }

        private void DispatchEvents(IReadOnlyCollection<IEvent> events)
        {
            var dispatched = _eventDispatcher.Dispatch(events);
            foreach (var pair in dispatched)
            {
                _eventSender.Push(pair.Key, pair.Value);
            }

            _telemetryPublisher.Publish<StatisticsEnqueuedOperationCountIdentity>(dispatched[StatisticsFlow.Instance].Count);
            _telemetryPublisher.Publish<AggregateEnqueuedOperationCountIdentity>(dispatched[AggregatesFlow.Instance].Count);
        }
    }
}