using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Confluent.Kafka;

using Newtonsoft.Json;
using NuClear.OperationsProcessing.Transports.Kafka;
using NuClear.Replication.Core;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.OperationsProcessing.AmsFactsFlow
{
    public sealed class AmsFactsCommandFactory : ICommandFactory<KafkaMessage>
    {
        public IReadOnlyCollection<ICommand> CreateCommands(KafkaMessage @event)
        {
            var message = @event.Message;

            return message.Value == null
                       ? CreateCommandFromHeartBeat(message)
                       : CreateCommandFromStateChange(message);
        }

        private IReadOnlyCollection<ICommand> CreateCommandFromHeartBeat(Message message)
            => new[] { new IncrementAmsStateCommand(new AmsState(message.Offset, message.Timestamp.UtcDateTime)) };

        private IReadOnlyCollection<ICommand> CreateCommandFromStateChange(Message message)
        {
            var value = JsonConvert.DeserializeObject<AdvertisementMessage>(Encoding.UTF8.GetString(message.Value));

            return new ICommand[]
                {
                        new IncrementAmsStateCommand(new AmsState(message.Offset, message.Timestamp.UtcDateTime)),
                        new ReplaceDataObjectCommand<Advertisement>(typeof(Advertisement),
                            new[] { new Advertisement { Id = value.Id, FirmId = value.FirmId, StateCode = value.StateCode } }),
                        new ReplaceDataObjectCommand<EntityName>(typeof(EntityName),
                            new[] { new EntityName { Id = value.Id, EntityType = EntityTypeIds.Advertisement, Name = value.Name } }),
                };
        }

        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private sealed class AdvertisementMessage
        {
            public long Id { get; set; }
            public long FirmId { get; set; }
            public string Name { get; set; }
            public int StateCode { get; set; }
        }
    }
}