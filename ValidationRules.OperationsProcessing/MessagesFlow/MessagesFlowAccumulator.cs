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
        private readonly ICommandFactory _commandFactory;

        public MessagesFlowAccumulator()
        {
            _commandFactory = new MessagesFlowCommandFactory();
        }

        protected override AggregatableMessage<ICommand> Process(EventMessage message)
        {
            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = _commandFactory.CreateCommands(message.Event).ToList()
            };
        }

        private sealed class MessagesFlowCommandFactory : ICommandFactory
        {
            public IEnumerable<ICommand> CreateCommands(IEvent @event)
            {
                switch (@event)
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
                        throw new ArgumentException($"Unexpected event '{@event}'", nameof(@event));
                }
            }
        }
    }
}