using System.Xml.Linq;

using Microsoft.ServiceBus.Messaging;

using NuClear.Messaging.API.Processing.Actors.Transformers;
using NuClear.Replication.Core;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus
{
    public sealed class BrokeredMessage2EventMessageTransformer : MessageTransformerBase<ServiceBusEventMessage, EventMessage>
    {
        private readonly IXmlEventSerializer _eventSerializer;

        public BrokeredMessage2EventMessageTransformer(IXmlEventSerializer eventSerializer)
        {
            _eventSerializer = eventSerializer;
        }

        protected override EventMessage Transform(ServiceBusEventMessage originalMessage)
            => new EventMessage(originalMessage.Id, originalMessage.EventHappenedTime, Deserialize(originalMessage.BrokeredMessage));

        private IEvent Deserialize(BrokeredMessage message)
            => _eventSerializer.Deserialize(message.GetBody<XElement>());
    }
}