using System.Collections.Generic;

using NuClear.OperationsProcessing.Transports.Kafka;
using NuClear.Replication.Core;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Dto;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.OperationsProcessing.Facts.AmsFactsFlow
{
    public sealed class AmsFactsCommandFactory : ICommandFactory<KafkaMessage>
    {
        private readonly IDeserializer<Confluent.Kafka.Message, AdvertisementDto> _deserializer;
        public AmsFactsCommandFactory()
        {
            _deserializer = new AdvertisementDtoDeserializer();
        }

        public IReadOnlyCollection<ICommand> CreateCommands(KafkaMessage kafkaMessage)
        {
            var commands = new List<ICommand>
                {
                    new IncrementAmsStateCommand(new AmsState(kafkaMessage.Message.Offset,
                                                              kafkaMessage.Message.Timestamp.UtcDateTime))
                };

            var dtos = _deserializer.Deserialize(kafkaMessage.Message);
            if (dtos.Count > 0)
            {
                commands.Add(new ReplaceDataObjectCommand(typeof(Advertisement), dtos));
            }

            return commands;
        }
    }
}