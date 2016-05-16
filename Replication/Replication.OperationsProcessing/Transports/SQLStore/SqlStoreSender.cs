using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.Messaging.API.Flows;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.Core;
using NuClear.Storage.API.Writings;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public sealed class SqlStoreSender : IEventSender
    {
        private readonly IIdentityGenerator _identityGenerator;
        private readonly IRepository<PerformedOperationFinalProcessing> _repository;
        private readonly IXmlEventSerializer _serializer;

        public SqlStoreSender(
            IIdentityGenerator identityGenerator,
            IRepository<PerformedOperationFinalProcessing> repository,
            IXmlEventSerializer serializer)
        {
            _identityGenerator = identityGenerator;
            _repository = repository;
            _serializer = serializer;
        }

        public void Push<TEvent, TFlow>(TFlow targetFlow, IReadOnlyCollection<TEvent> events)
            where TFlow : IMessageFlow
            where TEvent : IEvent
        {
            using (Probe.Create($"Send {typeof(TEvent).Name}"))
            {
                var transportMessages = events.Select(x => Serialize(x)).ToArray();
                Save(transportMessages, targetFlow.Id);
            }
        }

        private PerformedOperationFinalProcessing Serialize(IEvent @event)
            => new PerformedOperationFinalProcessing
                {
                    OperationId = Guid.NewGuid(),
                    Context = _serializer.Serialize(@event).ToString(SaveOptions.DisableFormatting)
                };

        private void Save(IReadOnlyCollection<PerformedOperationFinalProcessing> transportMessages, Guid targetFlowId)
        {
            foreach (var transportMessage in transportMessages)
            {
                transportMessage.Id = _identityGenerator.Next();
                transportMessage.CreatedOn = DateTime.UtcNow;
                transportMessage.MessageFlowId = targetFlowId;
            }

            _repository.AddRange(transportMessages);
            _repository.Save();
        }
    }
}