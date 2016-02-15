using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Flows;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Writings;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public sealed class SqlStoreSender<TOperation, TFlow> : IOperationSender<TOperation>
        where TOperation : IOperation
        where TFlow : MessageFlowBase<TFlow>, new()
    {
        private readonly IIdentityGenerator _identityGenerator;
        private readonly IRepository<PerformedOperationFinalProcessing> _repository;
        private readonly IOperationSerializer<TOperation> _serializer;
        private readonly TFlow _targetFlow;

        public SqlStoreSender(
            IIdentityGenerator identityGenerator,
            IRepository<PerformedOperationFinalProcessing> repository,
            IOperationSerializer<TOperation> serializer)
        {
            _identityGenerator = identityGenerator;
            _repository = repository;
            _serializer = serializer;
            _targetFlow = MessageFlowBase<TFlow>.Instance;
        }

        public void Push(IEnumerable<TOperation> operations)
        {
            using (Probe.Create($"Send {typeof(TOperation).Name}"))
            {
                var transportMessages = operations.Select(operation => _serializer.Serialize(operation, _targetFlow));
                Save(transportMessages.ToArray());
            }
        }

        private void Save(IReadOnlyCollection<PerformedOperationFinalProcessing> transportMessages)
        {
            foreach (var transportMessage in transportMessages)
            {
                transportMessage.Id = _identityGenerator.Next();
            }

            _repository.AddRange(transportMessages);
            _repository.Save();
        }
    }
}
