using System.Linq;
using System.Xml.Linq;

using NuClear.Messaging.API.Processing.Actors.Transformers;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.Core;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public sealed class SqlStoreMessage2EventMessageTransformer : MessageTransformerBase<PerformedOperationsFinalProcessingMessage, EventMessage>
    {
        private readonly IXmlEventSerializer _eventSerializer;

        public SqlStoreMessage2EventMessageTransformer(IXmlEventSerializer eventSerializer)
        {
            _eventSerializer = eventSerializer;
        }

        protected override EventMessage Transform(PerformedOperationsFinalProcessingMessage originalMessage)
            => new EventMessage(
                originalMessage.FinalProcessings.Single().OperationId,
                Deserialize(originalMessage.FinalProcessings.Single().Context));

        private IEvent Deserialize(string message)
            => _eventSerializer.Deserialize(XElement.Parse(message));
    }
}