using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsProcessing.Transports.Kafka;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
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

                var message = ((KafkaMessageEvent)@event).KafkaMessage.Message;
                commands.Add(new IncrementAmsStateCommand(new AmsState(message.Offset, message.Timestamp.UtcDateTime)));

                // filter heartbeat messages
                if (message.Value != null)
                {
                    var dtos = new List<AdvertisementDto> { JsonConvert.DeserializeObject<AdvertisementDto>(Encoding.UTF8.GetString(message.Value)) };
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