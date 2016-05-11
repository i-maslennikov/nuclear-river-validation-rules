using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Storage.Model.Statistics;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Commands;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Telemetry;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Final
{
    public sealed class ProjectStatisticsAggregateCommandsHandler : IMessageProcessingHandler
    {
        private readonly IAggregateActorFactory _aggregateActorFactory;
        private readonly ITelemetryPublisher _telemetryPublisher;
        private readonly ITracer _tracer;

        public ProjectStatisticsAggregateCommandsHandler(IAggregateActorFactory aggregateActorFactory, ITelemetryPublisher telemetryPublisher, ITracer tracer)
        {
            _aggregateActorFactory = aggregateActorFactory;
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
                foreach (var message in messages.Cast<AggregatableMessage<IAggregateCommand>>())
                {
                    var actor = _aggregateActorFactory.Create(typeof(ProjectStatistics));
                    actor.ExecuteCommands(message.Commands);

                    _telemetryPublisher.Publish<StatisticsProcessedOperationCountIdentity>(message.Commands.Count);
                    _telemetryPublisher.Publish<StatisticsProcessingDelayIdentity>((long)(DateTime.UtcNow - message.EventHappenedTime).TotalMilliseconds);
                }

                return MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error when calculating statistics");
                return MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }
    }
}