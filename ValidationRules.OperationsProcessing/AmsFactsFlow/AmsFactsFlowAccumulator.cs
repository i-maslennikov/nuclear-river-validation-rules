using System.Collections.Generic;
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
        private readonly ICommandFactory<KafkaMessage> _commandFactory;

        public AmsFactsFlowAccumulator()
        {
            _commandFactory = new AmsFactsCommandFactory();
        }

        protected override AggregatableMessage<ICommand> Process(KafkaMessage kafkaMessage)
        {
            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = _commandFactory.CreateCommands(kafkaMessage),
            };
        }

        private sealed class AmsFactsCommandFactory : ICommandFactory<KafkaMessage>
        {
            public IReadOnlyCollection<ICommand> CreateCommands(KafkaMessage kafkaMessage)
            {
                var commands = new List<ICommand>
                    { new IncrementAmsStateCommand(new AmsState(kafkaMessage.Message.Offset, kafkaMessage.Message.Timestamp.UtcDateTime)) };

                // filter heartbeat & tombstone messages
                var messagePayload = kafkaMessage.Message.Value;
                if (messagePayload != null)
                {
                    var dto = JsonConvert.DeserializeObject<AdvertisementDto>(Encoding.UTF8.GetString(messagePayload));
                    dto.Offset = kafkaMessage.Message.Offset;
                    commands.Add(new ReplaceDataObjectCommand(typeof(Advertisement), new List<AdvertisementDto> { dto }));
                }

                return commands;
            }
        }
    }
}