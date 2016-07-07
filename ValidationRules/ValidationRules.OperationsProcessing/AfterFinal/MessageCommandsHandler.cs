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
using NuClear.ValidationRules.OperationsProcessing.Identities.Telemetry;
using NuClear.ValidationRules.Replication.AccountRules.Validation;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.PriceRules.Validation;

namespace NuClear.ValidationRules.OperationsProcessing.AfterFinal
{
    public sealed class MessageCommandsHandler : IMessageProcessingHandler
    {
        private readonly ITelemetryPublisher _telemetryPublisher;
        private readonly ITracer _tracer;
        private readonly CreateNewVersionActor _createNewVersionActor;
        private readonly MinimumAdvertisementAmountActor _minimumAdvertisementAmountActor;
        private readonly MaximumAdvertisementAmountActor _maximumAdvertisementAmountActor;
        private readonly AdvertisementAmountRestrictionIntegrityActor _restrictionIntegrityActor;
        private readonly OrderPositionCorrespontToInactivePositionActor _orderPositionCorrespontToInactivePositionActor;
        private readonly OrderPositionDoesntCorrespontToActualPriceActor _orderPositionDoesntCorrespontToActualPriceActor;
        private readonly OrderPositionsDoesntCorrespontToActualPriceActor _orderPositionsDoesntCorrespontToActualPriceActor;
        private readonly AssociatedPositionsGroupCountActor _associatedPositionsGroupCountActor;
        private readonly DeniedPositionsCheckActor _deniedPositionsCheckActor;
        private readonly AssociatedPositionWithoutPrincipalActor _associatedPositionWithoutPrincipalActor;
        private readonly LinkedObjectsMissedInPrincipalsActor _linkedObjectsMissedInPrincipalsActor;
        private readonly ConflictingPrincipalPositionActor _conflictingPrincipalPositionActor;
        private readonly AccountShouldExistActor _accountShouldExistActor;
        private readonly LockShouldNotExistActor _lockShouldNotExistActor;
        private readonly AccountBalanceShouldBePositiveActor _accountBalanceShouldBePositiveActor;

        public MessageCommandsHandler(
            ITelemetryPublisher telemetryPublisher,
            ITracer tracer,
            CreateNewVersionActor createNewVersionActor,
            MinimumAdvertisementAmountActor minimumAdvertisementAmountActor,
            MaximumAdvertisementAmountActor maximumAdvertisementAmountActor,
            AdvertisementAmountRestrictionIntegrityActor restrictionIntegrityActor,
            OrderPositionCorrespontToInactivePositionActor orderPositionCorrespontToInactivePositionActor,
            OrderPositionDoesntCorrespontToActualPriceActor orderPositionDoesntCorrespontToActualPriceActor,
            OrderPositionsDoesntCorrespontToActualPriceActor orderPositionsDoesntCorrespontToActualPriceActor,
            AssociatedPositionsGroupCountActor associatedPositionsGroupCountActor,
            DeniedPositionsCheckActor deniedPositionsCheckActor,
            AssociatedPositionWithoutPrincipalActor associatedPositionWithoutPrincipalActor,
            LinkedObjectsMissedInPrincipalsActor linkedObjectsMissedInPrincipalsActor,
            ConflictingPrincipalPositionActor conflictingPrincipalPositionActor,
            AccountShouldExistActor accountShouldExistActor,
            LockShouldNotExistActor lockShouldNotExistActor,
            AccountBalanceShouldBePositiveActor accountBalanceShouldBePositiveActor)
        {
            _telemetryPublisher = telemetryPublisher;
            _tracer = tracer;
            _createNewVersionActor = createNewVersionActor;
            _minimumAdvertisementAmountActor = minimumAdvertisementAmountActor;
            _maximumAdvertisementAmountActor = maximumAdvertisementAmountActor;
            _restrictionIntegrityActor = restrictionIntegrityActor;
            _orderPositionCorrespontToInactivePositionActor = orderPositionCorrespontToInactivePositionActor;
            _orderPositionDoesntCorrespontToActualPriceActor = orderPositionDoesntCorrespontToActualPriceActor;
            _orderPositionsDoesntCorrespontToActualPriceActor = orderPositionsDoesntCorrespontToActualPriceActor;
            _associatedPositionsGroupCountActor = associatedPositionsGroupCountActor;
            _deniedPositionsCheckActor = deniedPositionsCheckActor;
            _associatedPositionWithoutPrincipalActor = associatedPositionWithoutPrincipalActor;
            _linkedObjectsMissedInPrincipalsActor = linkedObjectsMissedInPrincipalsActor;
            _conflictingPrincipalPositionActor = conflictingPrincipalPositionActor;
            _accountShouldExistActor = accountShouldExistActor;
            _lockShouldNotExistActor = lockShouldNotExistActor;
            _accountBalanceShouldBePositiveActor = accountBalanceShouldBePositiveActor;
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

            // Транзакция важна для запросов в пространстве Messages, запросы в PriceAggregate нужно выполнять без транзакции, хотя в идеале хотелось бы две независимые транзакции.
            var transactionOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero };
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                // Думаю, пока достаточно указывать вызваемые акторы явно, о фабриках подумаем позже.
                _minimumAdvertisementAmountActor.ExecuteCommands(commands);
                _maximumAdvertisementAmountActor.ExecuteCommands(commands);
                _restrictionIntegrityActor.ExecuteCommands(commands);
                _orderPositionCorrespontToInactivePositionActor.ExecuteCommands(commands);
                _orderPositionDoesntCorrespontToActualPriceActor.ExecuteCommands(commands);
                _orderPositionsDoesntCorrespontToActualPriceActor.ExecuteCommands(commands);
                _associatedPositionsGroupCountActor.ExecuteCommands(commands);
                _deniedPositionsCheckActor.ExecuteCommands(commands);
                _associatedPositionWithoutPrincipalActor.ExecuteCommands(commands);
                _linkedObjectsMissedInPrincipalsActor.ExecuteCommands(commands);
                _conflictingPrincipalPositionActor.ExecuteCommands(commands);
                _accountShouldExistActor.ExecuteCommands(commands);
                _lockShouldNotExistActor.ExecuteCommands(commands);
                _accountBalanceShouldBePositiveActor.ExecuteCommands(commands);

                // Задача: добиться того, чтобы все изменения попали в Version, содержащий токен состояния либо более ранний.
                // Этого легко достичь, просто вызвав обработчик команды CreateNewVersion последним в цепочке.
                // Благодаря этому все изменения, предшествующие состоянию erm будут гарантированно включены в версию проверок.
                _createNewVersionActor.ExecuteCommands(commands);

                transaction.Complete();
            }

            _telemetryPublisher.Publish<MessageProcessedOperationCountIdentity>(commands.Count);
        }
    }
}