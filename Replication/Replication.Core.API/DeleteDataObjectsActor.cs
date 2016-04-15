using System.Collections.Generic;
using System.Linq;

using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.API
{
    public sealed class DeleteDataObjectsActor<TDataObject> : DataObjectsActorBase<TDataObject>
        where TDataObject : class
    {
        private readonly IBulkRepository<TDataObject> _bulkRepository;
        private readonly IDataChangesHandler<TDataObject> _dataChangesHandler;

        public DeleteDataObjectsActor(
            IQuery query,
            IBulkRepository<TDataObject> bulkRepository,
            IStorageBasedDataObjectAccessor<TDataObject> storageBasedDataObjectAccessor,
            IDataChangesHandler<TDataObject> dataChangesHandler)
            : base(query, storageBasedDataObjectAccessor)
        {
            _bulkRepository = bulkRepository;
            _dataChangesHandler = dataChangesHandler;
        }

        public DeleteDataObjectsActor(
            IQuery query,
            IBulkRepository<TDataObject> bulkRepository,
            IStorageBasedDataObjectAccessor<TDataObject> storageBasedDataObjectAccessor)
            : this(query, bulkRepository, storageBasedDataObjectAccessor, new NullDataChangesHandler<TDataObject>())
        {
        }

        public override IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var changes = DetectChanges(commands);

            var toDelete = changes.Complement.ToArray();

            _bulkRepository.Delete(toDelete);
            return _dataChangesHandler.HandleDeletes(toDelete);
        }
    }
}