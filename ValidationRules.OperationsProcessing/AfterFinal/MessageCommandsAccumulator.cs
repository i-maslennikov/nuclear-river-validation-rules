using System;
using System.Collections.Generic;

using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.ValidationRules.OperationsProcessing.Identities.Flows;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.OperationsProcessing.AfterFinal
{
    public sealed class MessageCommandsAccumulator :
        MessageProcessingContextAccumulatorBase<MessagesFlow, EventMessage, AggregatableMessage<ICommand>>
    {
        protected override AggregatableMessage<ICommand> Process(EventMessage message)
        {
            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = CreateCommands(message.Event),
            };
        }

        private IReadOnlyCollection<ICommand> CreateCommands(IEvent @event)
        {
            var stateIncrementedEvent = @event as AggregatesStateIncrementedEvent;
            if (stateIncrementedEvent != null)
            {
                return new[] { new CreateNewVersionCommand(stateIncrementedEvent.IncludedTokens) };
            }

            var batchProcessedEvent = @event as AggregatesBatchProcessedEvent;
            if (batchProcessedEvent != null)
            {
                return new[] { new RecordDelayCommand(batchProcessedEvent.EventTime) };
            }

            var resultOutdatedEvent = @event as ResultOutdatedEvent;
            if (resultOutdatedEvent != null)
            {
                return new[] { new RecalculateValidationRuleCompleteCommand(resultOutdatedEvent.Rule) };
            }

            var resultPartiallyOutdatedEvent = @event as ResultPartiallyOutdatedEvent;
            if (resultPartiallyOutdatedEvent != null)
            {
                return new[] { new RecalculateValidationRulePartiallyCommand(resultPartiallyOutdatedEvent.Rule, resultPartiallyOutdatedEvent.OrderIds) };
            }

            throw new ArgumentException($"Unexpected event '{@event}'", nameof(@event));
        }
    }
}