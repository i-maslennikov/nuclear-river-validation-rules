using System;
using System.Collections.Generic;
using System.Transactions;

using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.Actors
{
    public sealed class ReplaceDataObjectsActor<TDataObject> : IActor
        where TDataObject : class
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<TDataObject> _bulkRepository;
        private readonly IMemoryBasedDataObjectAccessor<TDataObject> _memoryBasedDataObjectAccessor;
        private readonly IDataChangesHandler<TDataObject> _dataChangesHandler;

        public ReplaceDataObjectsActor(
            IQuery query,
            IBulkRepository<TDataObject> bulkRepository,
            IMemoryBasedDataObjectAccessor<TDataObject> memoryBasedDataObjectAccessor,
            IDataChangesHandler<TDataObject> dataChangesHandler)
        {
            _query = query;
            _bulkRepository = bulkRepository;
            _memoryBasedDataObjectAccessor = memoryBasedDataObjectAccessor;
            _dataChangesHandler = dataChangesHandler;
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
                    var findSpecification = _memoryBasedDataObjectAccessor.GetFindSpecification(command);
                    _bulkRepository.Delete(_query.For(findSpecification));

                    var dataObjects = _memoryBasedDataObjectAccessor.GetDataObjects(command);

                    _bulkRepository.Create(dataObjects);
                    events.AddRange(_dataChangesHandler.HandleCreates(dataObjects));

                    transaction.Complete();
                }
            }

            return events;
        }
    }
}