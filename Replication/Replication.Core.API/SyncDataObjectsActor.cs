using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.API.Facts;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.API
{
    public sealed class SyncDataObjectsActor<TDataObject> : DataObjectsActor<TDataObject>, ISyncDataObjectsActor
        where TDataObject : class
    {
        private readonly IQuery _query;
        private readonly IStorageBasedFactActor<TDataObject> _storageBasedFactActor;
        private readonly IBulkRepository<TDataObject> _bulkRepository;

        public SyncDataObjectsActor(IQuery query, IBulkRepository<TDataObject> bulkRepository, IStorageBasedFactActor<TDataObject> storageBasedFactActor)
            : base(query, storageBasedFactActor)
        {
            _query = query;
            _bulkRepository = bulkRepository;
            _storageBasedFactActor = storageBasedFactActor;
        }


        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var events = new List<IEvent>();

            var changes = DetectChanges(commands);

            var toCreate = changes.Difference.ToArray();
            var toUpdate = changes.Intersection.ToArray();
            var toDelete = changes.Complement.ToArray();

            _bulkRepository.Delete(toCreate);
            events.AddRange(_storageBasedFactActor.HandleDeletes(toDelete));

            _bulkRepository.Create(toCreate);
            events.AddRange(_storageBasedFactActor.HandleCreates(toCreate));

            events.AddRange(_storageBasedFactActor.HandleReferences(_query, toUpdate));
            _bulkRepository.Update(toCreate);
            events.AddRange(_storageBasedFactActor.HandleUpdates(toUpdate));

            return events;
        }
    }
}