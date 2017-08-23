using System.Collections.Generic;
using System.Linq;
using System.Text;

using Confluent.Kafka;

using Newtonsoft.Json;

using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.ValidationRules.OperationsProcessing.Transports.Kafka;
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
                return messages.Select(x =>
                {
                    var dto = JsonConvert.DeserializeObject<AdvertisementDto>(Encoding.UTF8.GetString(x.Value));
                    // Разве не нужно пересчитать EntityName?
                    return new ReplaceDataObjectCommand(typeof(Advertisement), dto);
                }).ToList();
            }
        }
    }
}