using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Flows;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Writings;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public sealed class SqlStoreSender : IOperationSender
    {
        private readonly IIdentityGenerator _identityGenerator;
        private readonly IRepository<PerformedOperationFinalProcessing> _repository;
        private readonly IOperationSerializer _serializer;

        public SqlStoreSender(
            IIdentityGenerator identityGenerator,
            IRepository<PerformedOperationFinalProcessing> repository,
            IOperationSerializer serializer)
        {
            _identityGenerator = identityGenerator;
            _repository = repository;
            _serializer = serializer;
        }

        public void Push<TOperation, TFlow>(IEnumerable<TOperation> operations, TFlow targetFlow)
            where TFlow : IMessageFlow
            where TOperation : IOperation
        {
            using (Probe.Create($"Send {typeof(TOperation).Name}"))
            {
                var transportMessages = operations.Select(x => _serializer.Serialize(x));
                Save(transportMessages.ToArray(), targetFlow.Id);
            }
        }

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