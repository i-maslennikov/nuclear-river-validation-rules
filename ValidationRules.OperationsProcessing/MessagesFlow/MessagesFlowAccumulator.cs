using System;
using System.Collections.Generic;

using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.OperationsProcessing.MessagesFlow
{
    public sealed class MessagesFlowAccumulator : MessageProcessingContextAccumulatorBase<MessagesFlow, EventMessage, AggregatableMessage<ICommand>>
    {
        private readonly ICommandFactory<EventMessage> _commandFactory;

        public MessagesFlowAccumulator()
        {
            _commandFactory = new MessagesFlowCommandFactory();
        }

        protected override AggregatableMessage<ICommand> Process(EventMessage message)
        {
            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = _commandFactory.CreateCommands(message)
            };
        }

        private sealed class MessagesFlowCommandFactory : ICommandFactory<EventMessage>
        {
            public IReadOnlyCollection<ICommand> CreateCommands(EventMessage message)
            {
                switch (message.Event)
                {
                    case AmsStateIncrementedEvent amsStateIncrementedEvent:
                        return new[] { new StoreAmsStateCommand(amsStateIncrementedEvent.State) };

                    case ErmStateIncrementedEvent ermStateIncrementedEvent:
                        return new[] { new StoreErmStateCommand(ermStateIncrementedEvent.States) };

                    case DelayLoggedEvent delayLoggedEvent:
                        return new[] { new LogDelayCommand(delayLoggedEvent.EventTime) };

                    case ResultOutdatedEvent resultOutdatedEvent:
                        return new[] { new RecalculateValidationRuleCompleteCommand(resultOutdatedEvent.Rule) };

                    case ResultPartiallyOutdatedEvent resultPartiallyOutdatedEvent:
                        return new[] { new RecalculateValidationRulePartiallyCommand(resultPartiallyOutdatedEvent.Rule, resultPartiallyOutdatedEvent.OrderIds) };

                    default:
                        throw new ArgumentException($"Unexpected event '{message.Event}'", nameof(message.Event));
                }
            }
        }
    }
}