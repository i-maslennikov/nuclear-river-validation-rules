using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.Telemetry;
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;
using NuClear.ValidationRules.OperationsProcessing.Telemetry;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Messages;

namespace NuClear.ValidationRules.OperationsProcessing.AfterFinal
{
    public sealed class MessageCommandsHandler : IMessageProcessingHandler
    {
        private readonly ITelemetryPublisher _telemetryPublisher;
        private readonly ITracer _tracer;
        private readonly ValidationRuleActor _validationRuleActor;

        public MessageCommandsHandler(
            ITelemetryPublisher telemetryPublisher,
            ITracer tracer,
            ValidationRuleActor validationRuleActor)
        {
            _telemetryPublisher = telemetryPublisher;
            _tracer = tracer;
            _validationRuleActor = validationRuleActor;
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            try
            {
                using (Probe.Create("ETL3 Transforming"))
                {
                    var commands = processingResultsMap.SelectMany(x => x.Value).Cast<AggregatableMessage<ICommand>>().SelectMany(x => x.Commands).ToList();

                    Handle(commands.OfType<IValidationRuleCommand>().ToList());
                    Handle(commands.OfType<LogDelayCommand>().ToList());

                    return processingResultsMap.Keys.Select(bucketId => MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded());
                }
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error when calculating validation rules");
                return processingResultsMap.Keys.Select(bucketId => MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex));
            }
        }

        private void Handle(IReadOnlyCollection<LogDelayCommand> commands)
        {
            if (!commands.Any())
            {
                return;
            }

            var eldestEventTime = commands.Min(x => x.EventTime);
            var delta = DateTime.UtcNow - eldestEventTime;
            _telemetryPublisher.Publish<MessageProcessingDelayIdentity>((long)delta.TotalMilliseconds);
        }

        private void Handle(IReadOnlyCollection<IValidationRuleCommand> commands)
        {
            if (!commands.Any())
            {
                return;
            }

            // Транзакция важна для запросов в пространстве Messages, запросы в Aggregates нужно выполнять без транзакции, хотя в идеале хотелось бы две независимые транзакции.
            var transactionOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero };
            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                using (Probe.Create("ValidationRuleActor"))
                {
                    _validationRuleActor.ExecuteCommands(commands);
                }

                scope.Complete();
            }

            _telemetryPublisher.Publish<MessageProcessedOperationCountIdentity>(commands.Count);
        }
    }
}