using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Commands;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Telemetry;
using NuClear.Telemetry;
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Final
{
    public sealed class AggregateCommandsHandler : IMessageProcessingHandler
    {
        private readonly IAggregateActorFactory _aggregateActorFactory;
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public AggregateCommandsHandler(IAggregateActorFactory aggregateActorFactory, ITracer tracer, ITelemetryPublisher telemetryPublisher)
        {
            _aggregateActorFactory = aggregateActorFactory;
            _tracer = tracer;
            _telemetryPublisher = telemetryPublisher;
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
                    var commandGroups = message.Commands.GroupBy(x => x.AggregateRootType);

                    // TODO: Can agreggate actors be executed in parallel? See https://github.com/2gis/nuclear-river/issues/76
                    foreach (var commandGroup in commandGroups)
                    {
                        ExecuteCommands(commandGroup.Key, commandGroup.ToArray());
                    }

                    _telemetryPublisher.Publish<AggregateProcessedOperationCountIdentity>(message.Commands.Count);
                    _telemetryPublisher.Publish<AggregateProcessingDelayIdentity>((long)(DateTime.UtcNow - message.EventHappenedTime).TotalMilliseconds);
                }

                return MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error when calculating aggregates");
                return MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }

        private void ExecuteCommands(Type aggregateRootType, IReadOnlyCollection<IAggregateCommand> commands)
        {
            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
            {
                using (Probe.Create($"ETL2 {aggregateRootType.Name}"))
                {
                    var actor = _aggregateActorFactory.Create(aggregateRootType);
                    actor.ExecuteCommands(commands);
                }

                transaction.Complete();
            }
        }
    }
}