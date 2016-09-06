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
using NuClear.Tracing.API;
using NuClear.ValidationRules.OperationsProcessing.Telemetry;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.PriceRules.Validation;

namespace NuClear.ValidationRules.OperationsProcessing.AfterFinal
{
    public sealed class MessageCommandsHandler : IMessageProcessingHandler
    {
        private readonly ITelemetryPublisher _telemetryPublisher;
        private readonly ITracer _tracer;
        private readonly CreateNewVersionActor _createNewVersionActor;
        private readonly ValidationRuleActor _validationRuleActor;

        public MessageCommandsHandler(
            ITelemetryPublisher telemetryPublisher,
            ITracer tracer,
            CreateNewVersionActor createNewVersionActor,
            ValidationRuleActor validationRuleActor)
        {
            _telemetryPublisher = telemetryPublisher;
            _tracer = tracer;
            _createNewVersionActor = createNewVersionActor;
            _validationRuleActor = validationRuleActor;
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            try
            {
                var messages = processingResultsMap.SelectMany(pair => pair.Value)
                                                   .Cast<AggregatableMessage<ICommand>>()
                                                   .ToArray();

                Handle(messages.SelectMany(x => x.Commands).OfType<IValidationRuleCommand>().ToArray());
                Handle(messages.SelectMany(x => x.Commands).OfType<RecordDelayCommand>().ToArray());

                return processingResultsMap.Keys.Select(bucketId => MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded());
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error when calculating validation rules");
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
            _telemetryPublisher.Publish<MessageProcessingDelayIdentity>((long)delta.TotalMilliseconds);
        }

        private void Handle(IReadOnlyCollection<IValidationRuleCommand> commands)
        {
            if (!commands.Any())
            {
                return;
            }

            // Транзакция важна для запросов в пространстве Messages, запросы в PriceAggregates нужно выполнять без транзакции, хотя в идеале хотелось бы две независимые транзакции.
            var transactionOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero };
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                // Задача: добиться того, чтобы все изменения попали в Version, содержащий токен состояния либо более ранний.
                // Этого легко достичь, просто вызвав обработчик команды CreateNewVersion последним в цепочке.
                // Благодаря этому все изменения, предшествующие состоянию erm будут гарантированно включены в версию проверок.
                _validationRuleActor.ExecuteCommands(commands);
                _createNewVersionActor.ExecuteCommands(commands);

                transaction.Complete();
            }

            _telemetryPublisher.Publish<MessageProcessedOperationCountIdentity>(commands.Count);
        }
    }
}