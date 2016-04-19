using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.Replication.OperationsProcessing;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.ValidationRules.OperationsProcessing.Final
{
    public sealed class AggregateOperationAggregatableMessageHandler : IMessageProcessingHandler
    {
        private readonly IAggregatesConstructor _aggregatesConstructor;
        private readonly ITelemetryPublisher _telemetryPublisher;
        private readonly ITracer _tracer;

        public AggregateOperationAggregatableMessageHandler(IAggregatesConstructor aggregatesConstructor, ITelemetryPublisher telemetryPublisher, ITracer tracer)
        {
            _aggregatesConstructor = aggregatesConstructor;
            _telemetryPublisher = telemetryPublisher;
            _tracer = tracer;
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            return processingResultsMap.Select(pair => Handle(pair.Key, pair.Value));
        }

        private StageResult Handle(Guid bucketId, IEnumerable<IAggregatableMessage> messages)
        {
            try
            {
                foreach (var message in messages.OfType<OperationAggregatableMessage<AggregateOperation>>())
                {
                    _aggregatesConstructor.Execute(message.Operations);

                    // TODO: telemetry
                    //_telemetryPublisher.Publish<AggregateProcessedOperationCountIdentity>(message.Operations.Count);

                    //_telemetryPublisher.Publish<AggregateProcessingDelayIdentity>((long)(DateTime.UtcNow - message.OperationTime).TotalMilliseconds);
                }

                return MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error when calculating aggregates");
                return MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }
    }
}