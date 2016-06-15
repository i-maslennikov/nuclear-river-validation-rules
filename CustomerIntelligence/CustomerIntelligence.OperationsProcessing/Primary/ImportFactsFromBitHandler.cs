using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing.Primary;
using NuClear.Replication.OperationsProcessing.Telemetry;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromBitHandler : IMessageProcessingHandler
    {
        private readonly IDataObjectsActorFactory _dataObjectsActorFactory;
        private readonly IEventSender _eventSender;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public ImportFactsFromBitHandler(
            IDataObjectsActorFactory dataObjectsActorFactory,
            IEventSender eventSender,
            IEventDispatcher eventDispatcher,
            ITelemetryPublisher telemetryPublisher,
            ITracer tracer)
        {
            _dataObjectsActorFactory = dataObjectsActorFactory;
            _eventSender = eventSender;
            _tracer = tracer;
            _telemetryPublisher = telemetryPublisher;
            _eventDispatcher = eventDispatcher;
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            return processingResultsMap.Select(pair => Handle(pair.Key, pair.Value.OfType<CorporateBusAggregatableMessage>()));
        }

        private StageResult Handle(Guid bucketId, IEnumerable<CorporateBusAggregatableMessage> messages)
        {
            try
            {
                foreach (var message in messages)
                {
                    var actors = _dataObjectsActorFactory.Create();
                    foreach (var actor in actors)
                    {
                        var events = actor.ExecuteCommands(message.Commands);
                        DispatchOperations(events);
                    }

                    _telemetryPublisher.Publish<BitStatisticsEntityProcessedCountIdentity>(message.Commands.Count);
                }

                return MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error when import facts for BIT");
                return MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }

        private void DispatchOperations(IReadOnlyCollection<IEvent> events)
        {
            foreach (var pair in _eventDispatcher.Dispatch(events))
            {
                _eventSender.Push(pair.Key, pair.Value);
            }
        }
    }
}