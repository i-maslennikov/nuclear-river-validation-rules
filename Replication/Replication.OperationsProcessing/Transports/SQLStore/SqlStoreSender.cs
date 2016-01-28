using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Common.Metadata.Model.Operations;
using NuClear.Messaging.API.Flows;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Storage.API.Writings;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public sealed class SqlStoreSender : IOperationSender
    {
        private readonly IIdentityGenerator _identityGenerator;
        private readonly IRepository<PerformedOperationFinalProcessing> _repository;
        private readonly IOperationSerializer<AggregateOperation> _serializer;

        public SqlStoreSender(
            IIdentityGenerator identityGenerator,
            IRepository<PerformedOperationFinalProcessing> repository,
            IOperationSerializer<AggregateOperation> serializer)
        {
            _identityGenerator = identityGenerator;
            _repository = repository;
            _serializer = serializer;
        }

        public void Push<TOperation, TFlow>(IEnumerable<TOperation> operations, TFlow targetFlow)
            where TFlow : MessageFlowBase<TFlow>, new()
            where TOperation : AggregateOperation
        {
            using (Probe.Create($"Send {typeof(TOperation).Name}"))
            {
                var transportMessages = operations.Select(operation => _serializer.Serialize(operation, targetFlow));
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
