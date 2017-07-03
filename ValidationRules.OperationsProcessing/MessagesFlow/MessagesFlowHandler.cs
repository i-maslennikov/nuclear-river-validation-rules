using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Messages;

namespace NuClear.ValidationRules.OperationsProcessing.MessagesFlow
{
    public sealed class MessagesFlowHandler : IMessageProcessingHandler
    {
        private readonly MessagesFlowTelemetryPublisher _telemetryPublisher;
        private readonly ITracer _tracer;
        private readonly ValidationRuleActor _validationRuleActor;
        private readonly TransactionOptions _transactionOptions;

        public MessagesFlowHandler(
            MessagesFlowTelemetryPublisher telemetryPublisher,
            ITracer tracer,
            ValidationRuleActor validationRuleActor)
        {
            _telemetryPublisher = telemetryPublisher;
            _tracer = tracer;
            _validationRuleActor = validationRuleActor;
            _transactionOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero };
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {

            try
            {
                using (Probe.Create("ETL3 Transforming"))
                // Транзакция важна для запросов в пространстве Messages, запросы в Aggregates нужно выполнять без транзакции, хотя в идеале хотелось бы две независимые транзакции.
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, _transactionOptions))
                {
                    var commands = processingResultsMap.SelectMany(x => x.Value).Cast<AggregatableMessage<ICommand>>().SelectMany(x => x.Commands).ToList();

                    Handle(commands.OfType<IValidationRuleCommand>().ToList());
                    Handle(commands.OfType<LogDelayCommand>().ToList());

                    transaction.Complete();
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
            _telemetryPublisher.Delay((int)delta.TotalMilliseconds);
        }

        private void Handle(IReadOnlyCollection<IValidationRuleCommand> commands)
        {
            if (!commands.Any())
            {
                return;
            }

            using (Probe.Create("ValidationRuleActor"))
            {
                _validationRuleActor.ExecuteCommands(commands);
            }
        }
    }
}