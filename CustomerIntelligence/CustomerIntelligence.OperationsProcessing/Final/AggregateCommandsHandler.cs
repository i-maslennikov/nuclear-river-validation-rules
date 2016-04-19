using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Identities.Telemetry;
using NuClear.Telemetry;
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Final
{
    public sealed class AggregateCommandsHandler : IMessageProcessingHandler
    {
        private readonly IAggregateCommandActorFactory _aggregateCommandActorFactory;
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public AggregateCommandsHandler(IAggregateCommandActorFactory aggregateCommandActorFactory, ITracer tracer, ITelemetryPublisher telemetryPublisher)
        {
            _aggregateCommandActorFactory = aggregateCommandActorFactory;
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
                foreach (var message in messages.Cast<OperationAggregatableMessage<IAggregateCommand>>())
                {
                    var commandGroups = message.Commands
                                               .GroupBy(x => new { CommandType = x.GetType(), x.AggregateRootType })
                                               .OrderByDescending(x => x.Key.CommandType, new AggregateCommandPriorityComparer());
                    foreach (var commandGroup in commandGroups)
                    {
                        ExecuteCommands(commandGroup.Key.CommandType, commandGroup.Key.AggregateRootType, commandGroup.ToArray());
                    }

                    _telemetryPublisher.Publish<AggregateProcessedOperationCountIdentity>(message.Commands.Count);
                    _telemetryPublisher.Publish<AggregateProcessingDelayIdentity>((long)(DateTime.UtcNow - message.OperationTime).TotalMilliseconds);
                }

                return MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error when calculating aggregates");
                return MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }

        private void ExecuteCommands(Type commandType, Type aggregateRootType, IReadOnlyCollection<IAggregateCommand> commands)
        {
            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
            {
                using (Probe.Create($"ETL2 {commandType.Name} {aggregateRootType.Name}"))
                {
                    var actor = _aggregateCommandActorFactory.Create(commandType, aggregateRootType);
                    actor.ExecuteCommands(commands);
                }

                transaction.Complete();
            }
        }
    }
}