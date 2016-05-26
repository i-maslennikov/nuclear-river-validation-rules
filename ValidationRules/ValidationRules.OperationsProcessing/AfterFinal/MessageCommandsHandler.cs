using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.OperationsProcessing;
using NuClear.Telemetry;
using NuClear.Tracing.API;
using NuClear.ValidationRules.OperationsProcessing.Identities.Telemetry;
using NuClear.ValidationRules.Replication.Commands;

namespace NuClear.ValidationRules.OperationsProcessing.AfterFinal
{
    public sealed class MessageCommandsHandler : IMessageProcessingHandler
    {
        private readonly ITelemetryPublisher _telemetryPublisher;
        private readonly ITracer _tracer;

        public MessageCommandsHandler(ITelemetryPublisher telemetryPublisher, ITracer tracer)
        {
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
                foreach (var message in messages.OfType<AggregatableMessage<IValidationRuleCommand>>())
                {
                    // todo: передать сообщения бизнес-логике

                    // Идея какая? Нужно обрабатывать поток команд на вызов проверок правил. Хоть пакетом, хоть поодиночке.
                    // Но при порлучении команды CreateNewVersionCommand все насчитанные результаты должны быть зафиксированы в агрегате Version, который больше меняться не будет.
                    // В этот же экземпляр агрегата помещаем токены, пришедшие с командой.
                    // Все последующие команды должны приводить к изменениям в новом экземпляре Version и это будет происходить до следующей команды CreateNewVersionCommand.

                    _telemetryPublisher.Publish<MessageProcessedOperationCountIdentity>(message.Commands.Count);
                    _telemetryPublisher.Publish<MessageProcessingDelayIdentity>((long)(DateTime.UtcNow - message.EventHappenedTime).TotalMilliseconds);
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