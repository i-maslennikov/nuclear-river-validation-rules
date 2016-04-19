using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Model.Statistics;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Identities.Telemetry;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Final
{
    public sealed class ProjectStatisticsAggregateCommandsHandler : IMessageProcessingHandler
    {
        private readonly IAggregateCommandActorFactory _aggregateCommandActorFactory;
        private readonly ITelemetryPublisher _telemetryPublisher;
        private readonly ITracer _tracer;

        public ProjectStatisticsAggregateCommandsHandler(IAggregateCommandActorFactory aggregateCommandActorFactory, ITelemetryPublisher telemetryPublisher, ITracer tracer)
        {
            _aggregateCommandActorFactory = aggregateCommandActorFactory;
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
                foreach (var message in messages.Cast<OperationAggregatableMessage<IAggregateCommand>>())
                {
                    var commandGroups = message.Commands.GroupBy(x => x.GetType());
                    foreach (var commandGroup in commandGroups)
                    {
                        var actor = _aggregateCommandActorFactory.Create(commandGroup.Key, typeof(ProjectStatistics));
                        var commands = commandGroup.ToArray();

                        actor.ExecuteCommands(commands);

                        _telemetryPublisher.Publish<StatisticsProcessedOperationCountIdentity>(commands.Length);
                        _telemetryPublisher.Publish<StatisticsProcessingDelayIdentity>((long)(DateTime.UtcNow - message.OperationTime).TotalMilliseconds);
                    }
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