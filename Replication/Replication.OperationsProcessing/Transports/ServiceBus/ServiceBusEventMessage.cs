using System;

using Microsoft.ServiceBus.Messaging;

using NuClear.Messaging.API;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus
{
    public sealed class ServiceBusEventMessage : MessageBase
    {
        public ServiceBusEventMessage(BrokeredMessage brokeredMessagese)
        {
            BrokeredMessage = brokeredMessagese;
        }

        public BrokeredMessage BrokeredMessage { get; }

        public override Guid Id
            => Guid.Parse(BrokeredMessage.MessageId);

        public DateTime EventHappenedTime
            => (DateTime)BrokeredMessage.Properties["CreatedOn"];

        public static ServiceBusEventMessage Create<T>(Guid id, T body)
        {
            var message = new BrokeredMessage(body);
            message.Properties["CreatedOn"] = DateTime.UtcNow;
            return new ServiceBusEventMessage(message);
        }
    }
}