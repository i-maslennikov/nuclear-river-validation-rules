using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.OperationsProcessing.MessagesFlow
{
    public sealed class MessagesFlowAccumulator : MessageProcessingContextAccumulatorBase<MessagesFlow, EventMessage, AggregatableMessage<ICommand>>
    {
        protected override AggregatableMessage<ICommand> Process(EventMessage message)
        {
            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = CommandFactory.CreateCommands(message.Event).ToList()
            };
        }

        private static class CommandFactory
        {
            public static IEnumerable<ICommand> CreateCommands(IEvent @event)
            {
                var stateIncrementedEvent = @event as AggregatesStateIncrementedEvent;
                if (stateIncrementedEvent != null)
                {
                    return new[] { new CreateNewVersionCommand(stateIncrementedEvent.IncludedTokens) };
                }

                var aggregatesDelayLoggedEvent = @event as AggregatesDelayLoggedEvent;
                if (aggregatesDelayLoggedEvent != null)
                {
                    return new[] { new LogDelayCommand(aggregatesDelayLoggedEvent.EventTime) };
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
}