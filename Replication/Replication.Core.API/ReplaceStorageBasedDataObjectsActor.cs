using System.Collections.Generic;
using System.Linq;

using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.API
{
    public sealed class ReplaceStorageBasedDataObjectsActor<TDataObject> : DataObjectsActorBase<TDataObject>
        where TDataObject : class
    {
        private readonly IBulkRepository<TDataObject> _bulkRepository;
        private readonly IDataChangesHandler<TDataObject> _dataChangesHandler;

        public ReplaceStorageBasedDataObjectsActor(
            IQuery query,
            IBulkRepository<TDataObject> bulkRepository,
            IStorageBasedDataObjectAccessor<TDataObject> storageBasedDataObjectAccessor,
            IDataChangesHandler<TDataObject> dataChangesHandler)
            : base(query, storageBasedDataObjectAccessor)
        {
            _bulkRepository = bulkRepository;
            _dataChangesHandler = dataChangesHandler;
        }

        public ReplaceStorageBasedDataObjectsActor(
            IQuery query,
            IBulkRepository<TDataObject> bulkRepository,
            IStorageBasedDataObjectAccessor<TDataObject> storageBasedDataObjectAccessor)
            : this(query, bulkRepository, storageBasedDataObjectAccessor, new NullDataChangesHandler<TDataObject>())
        {
        }

        public override IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var events = new List<IEvent>();
            var changes = DetectChanges(commands);

            var toCreate = changes.Difference.ToArray();
            var toDelete = changes.Complement.ToArray();

            _bulkRepository.Delete(toDelete);
            events.AddRange(_dataChangesHandler.HandleDeletes(toDelete));

            _bulkRepository.Create(toCreate);
            events.AddRange(_dataChangesHandler.HandleCreates(toCreate));

            return events;
        }
    }
}