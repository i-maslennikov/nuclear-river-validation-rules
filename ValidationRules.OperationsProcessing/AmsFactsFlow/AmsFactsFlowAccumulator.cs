using System.Collections.Generic;
using System.Text;

using Confluent.Kafka;

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

        protected override AggregatableMessage<ICommand> Process(KafkaMessage message)
        {
            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = _commandFactory.CreateCommands(message),
            };
        }

        private sealed class AmsFactsCommandFactory : ICommandFactory<KafkaMessage>
        {
            public IReadOnlyCollection<ICommand> CreateCommands(KafkaMessage @event)
            {
                var message = @event.Message;

                return message.Value == null
                           ? CreateCommandFromHeartBeat(message)
                           : CreateCommandFromStateChange(message);
            }

            private IReadOnlyCollection<ICommand> CreateCommandFromHeartBeat(Message message)
                => new [] { new IncrementAmsStateCommand(new AmsState(message.Offset, message.Timestamp.UtcDateTime)) };

            private IReadOnlyCollection<ICommand> CreateCommandFromStateChange(Message message)
            {
                var dtos = new[] { JsonConvert.DeserializeObject<AdvertisementDto>(Encoding.UTF8.GetString(message.Value)) };
                return new ICommand[]
                    {
                        new IncrementAmsStateCommand(new AmsState(message.Offset, message.Timestamp.UtcDateTime)),
                        new ReplaceDataObjectCommand(typeof(Advertisement), dtos),
                    };
            }
        }
    }
}