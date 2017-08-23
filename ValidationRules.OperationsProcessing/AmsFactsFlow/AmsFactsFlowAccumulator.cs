using System.Collections.Generic;
using System.Text;

using Confluent.Kafka;

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
        protected override AggregatableMessage<ICommand> Process(KafkaMessage message)
        {
            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = CommandFactory.CreateCommands(message.Messages),
            };
        }

        private static class CommandFactory
        {
            public static IReadOnlyCollection<ICommand> CreateCommands(IReadOnlyCollection<Message> messages)
            {
                var commands = new List<ICommand>();

                // пока что хардкод для advertisement и heartbeat
                var dtos = new List<AdvertisementDto>();
                foreach (var message in messages)
                {
                    if (message.Value != null)
                    {
                        dtos.Add(JsonConvert.DeserializeObject<AdvertisementDto>(Encoding.UTF8.GetString(message.Value)));
                    }

                    commands.Add(new IncrementAmsStateCommand(new AmsState(message.Offset, message.Timestamp.UtcDateTime)));
                }

                commands.Add(new ReplaceDataObjectCommand(typeof(Advertisement), dtos));

                return commands;
            }
        }
    }
}