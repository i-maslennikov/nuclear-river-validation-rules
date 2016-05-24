using Microsoft.ServiceBus.Messaging;

using NuClear.OperationsLogging.Transports.ServiceBus;
using NuClear.Replication.Core;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus
{
    public sealed class Event2BrokeredMessageConverter : IEvent2BrokeredMessageConverter<IEvent>
    {
        private readonly IXmlEventSerializer _serializer;

        public Event2BrokeredMessageConverter(IXmlEventSerializer serializer)
        {
            _serializer = serializer;
        }

        public BrokeredMessage Convert(IEvent x)
            => new BrokeredMessage(_serializer.Serialize(x));
    }
}