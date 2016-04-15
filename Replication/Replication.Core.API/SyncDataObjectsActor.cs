using System.Collections.Generic;
using System.Linq;

using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.API
{
    public sealed class SyncDataObjectsActor<TDataObject> : DataObjectsActorBase<TDataObject>
        where TDataObject : class
    {
        private readonly IQuery _query;
        private readonly IDataChangesHandler<TDataObject> _dataChangesHandler;
        private readonly IBulkRepository<TDataObject> _bulkRepository;

        public SyncDataObjectsActor(
            IQuery query,
            IBulkRepository<TDataObject> bulkRepository,
            IStorageBasedDataObjectAccessor<TDataObject> storageBasedDataObjectAccessor,
            IDataChangesHandler<TDataObject> dataChangesHandler)
            : base(query, storageBasedDataObjectAccessor)
        {
            _query = query;
            _bulkRepository = bulkRepository;
            _dataChangesHandler = dataChangesHandler;
        }

        public SyncDataObjectsActor(
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
            var toUpdate = changes.Intersection.ToArray();
            var toDelete = changes.Complement.ToArray();

            _bulkRepository.Delete(toCreate);
            events.AddRange(_dataChangesHandler.HandleDeletes(toDelete));

            _bulkRepository.Create(toCreate);
            events.AddRange(_dataChangesHandler.HandleCreates(toCreate));

            events.AddRange(_dataChangesHandler.HandleReferences(_query, toUpdate));
            _bulkRepository.Update(toCreate);
            events.AddRange(_dataChangesHandler.HandleUpdates(toUpdate));

            return events;
        }
    }
}