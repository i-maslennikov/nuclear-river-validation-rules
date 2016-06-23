using System.Xml.Linq;

using Microsoft.ServiceBus.Messaging;

using NuClear.Messaging.API.Processing.Actors.Transformers;
using NuClear.OperationsProcessing.Transports.ServiceBus;
using NuClear.Replication.Core;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus
{
    public sealed class BrokeredMessageWithXmlPayload2EventMessageTransformer : MessageTransformerBase<BrokeredMessageDecorator, EventMessage>
    {
        private readonly IXmlEventSerializer _eventSerializer;

        public BrokeredMessageWithXmlPayload2EventMessageTransformer(IXmlEventSerializer eventSerializer)
        {
            _eventSerializer = eventSerializer;
        }

        protected override EventMessage Transform(BrokeredMessageDecorator originalMessage)
            => new EventMessage(originalMessage.Id, Deserialize(originalMessage.Message));

        private IEvent Deserialize(BrokeredMessage message)
            => _eventSerializer.Deserialize(message.GetBody<XElement>());
    }
}