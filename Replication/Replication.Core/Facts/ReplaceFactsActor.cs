using System;
using System.Collections.Generic;
using System.Transactions;

using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Facts;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.Facts
{
    public sealed class ReplaceFactsActor<TDataObject> : IActor
        where TDataObject : class
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<TDataObject> _bulkRepository;
        private readonly IMemoryBasedFactActor<TDataObject> _memoryBasedFactActor;

        public ReplaceFactsActor(IQuery query, IBulkRepository<TDataObject> bulkRepository, IMemoryBasedFactActor<TDataObject> memoryBasedFactActor)
        {
            _query = query;
            _bulkRepository = bulkRepository;
            _memoryBasedFactActor = memoryBasedFactActor;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var events = new List<IEvent>();
            foreach (var command in commands)
            {
                using (var transaction = new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
                {
                    var findSpecification = _memoryBasedFactActor.GetDataObjectsFindSpecification(command);
                    _bulkRepository.Delete(_query.For(findSpecification));

                    var dataObjects = _memoryBasedFactActor.GetDataObjects(command);
                    _bulkRepository.Create(dataObjects);

                    events.AddRange(_memoryBasedFactActor.HandleChanges(dataObjects));
                    transaction.Complete();
                }
            }

            return events;
        }
    }
}