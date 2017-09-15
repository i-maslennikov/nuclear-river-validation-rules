using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.ValidationRules.OperationsProcessing.Transports.Kafka;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Dto;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.OperationsProcessing.AmsFactsFlow
{
    public sealed class AmsFactsFlowAccumulator : MessageProcessingContextAccumulatorBase<AmsFactsFlow, KafkaMessage, AggregatableMessage<ICommand>>
    {
        private readonly ICommandFactory _commandFactory;

        public AmsFactsFlowAccumulator()
        {
            _commandFactory = new AmsFactsCommandFactory();
        }

        protected override AggregatableMessage<ICommand> Process(KafkaMessage message)
        {
            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = _commandFactory.CreateCommands(new KafkaMessageEvent(message)).ToList(),
            };
        }

        private sealed class AmsFactsCommandFactory : ICommandFactory
        {
            public IEnumerable<ICommand> CreateCommands(IEvent @event)
            {
                var commands = new List<ICommand>();
                var dtos = new List<AdvertisementDto>();

                var messages = ((KafkaMessageEvent)@event).KafkaMessage.Messages;
                foreach (var message in messages)
                {
                    commands.Add(new IncrementAmsStateCommand(new AmsState(message.Offset, message.Timestamp.UtcDateTime)));

                    // filter heartbeat messages
                    if (message.Value != null)
                    {
                        dtos.Add(JsonConvert.DeserializeObject<AdvertisementDto>(Encoding.UTF8.GetString(message.Value)));
                    }
                }

                if (dtos.Count != 0)
                {
                    commands.Add(new ReplaceDataObjectCommand(typeof(Advertisement), dtos));
                }

                return commands;
            }
        }

        private sealed class KafkaMessageEvent : IEvent
        {
            public KafkaMessageEvent(KafkaMessage kafkaMessage)
            {
                KafkaMessage = kafkaMessage;
            }

            public KafkaMessage KafkaMessage { get; }
        }
    }
}