using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.Core.Commands;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;
using NuClear.ValidationRules.OperationsProcessing.Identities.Flows;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.OperationsProcessing.AfterFinal
{
    public sealed class MessageEventsAccumulator :
        MessageProcessingContextAccumulatorBase<MessagesFlow, PerformedOperationsFinalProcessingMessage, AggregatableMessage<IValidationRuleCommand>>
    {
        private readonly IEventSerializer _serializer;

        public MessageEventsAccumulator(IEventSerializer serializer)
        {
            _serializer = serializer;
        }

        protected override AggregatableMessage<IValidationRuleCommand> Process(PerformedOperationsFinalProcessingMessage message)
        {
            var events = message.FinalProcessings.Select(x => _serializer.Deserialize(x)).ToArray();
            var commands = new List<IValidationRuleCommand>();
            foreach (var @event in events)
            {
                var stateIncrementedEvent = @event as StateIncrementedEvent;
                if (stateIncrementedEvent != null)
                {
                    commands.Add(new CreateNewVersionCommand(stateIncrementedEvent.IncludedTokens));
                    continue;
                }
            }

            return new AggregatableMessage<IValidationRuleCommand>
                {
                    TargetFlow = MessageFlow,
                    Commands = commands,
                    EventHappenedTime = message.FinalProcessings.Min(x => x.CreatedOn)
                };
        }
    }
}