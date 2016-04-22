using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.API.Commands;
using NuClear.Replication.Core.API.DataObjects;
using NuClear.Replication.Core.API.Equality;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.API.Actors
{
    public sealed class SyncDataObjectsActor<TDataObject> : DataObjectsActorBase<TDataObject>
        where TDataObject : class
    {
        private readonly IDataChangesHandler<TDataObject> _dataChangesHandler;
        private readonly IBulkRepository<TDataObject> _bulkRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;

        public SyncDataObjectsActor(
            IQuery query,
            IBulkRepository<TDataObject> bulkRepository,
            IEqualityComparerFactory equalityComparerFactory,
            IStorageBasedDataObjectAccessor<TDataObject> storageBasedDataObjectAccessor,
            IDataChangesHandler<TDataObject> dataChangesHandler)
            : base(query, storageBasedDataObjectAccessor)
        {
            _bulkRepository = bulkRepository;
            _equalityComparerFactory = equalityComparerFactory;
            _dataChangesHandler = dataChangesHandler;
        }

        public SyncDataObjectsActor(
            IQuery query,
            IBulkRepository<TDataObject> bulkRepository,
            IEqualityComparerFactory equalityComparerFactory,
            IStorageBasedDataObjectAccessor<TDataObject> storageBasedDataObjectAccessor)
            : this(query, bulkRepository, equalityComparerFactory, storageBasedDataObjectAccessor, new NullDataChangesHandler<TDataObject>())
        {
        }

        public override IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var commandsToExecute = commands.OfType<SyncDataObjectCommandBase>()
                                            .Where(x => x.DataObjectType == typeof(TDataObject))
                                            .ToArray();

            var events = new List<IEvent>();

            var changes = DetectChanges(commandsToExecute,
                                        _equalityComparerFactory.CreateIdentityComparer<TDataObject>(),
                                        _equalityComparerFactory.CreateCompleteComparer<TDataObject>());

            var toCreate = changes.Difference.ToArray();
            var toUpdate = changes.Intersection.ToArray();
            var toDelete = changes.Complement.ToArray();

            _bulkRepository.Delete(toCreate);
            events.AddRange(_dataChangesHandler.HandleDeletes(toDelete));
            events.AddRange(_dataChangesHandler.HandleRelates(toDelete));

            _bulkRepository.Create(toCreate);
            events.AddRange(_dataChangesHandler.HandleCreates(toCreate));
            events.AddRange(_dataChangesHandler.HandleRelates(toCreate));

            events.AddRange(_dataChangesHandler.HandleRelates(toUpdate));
            _bulkRepository.Update(toCreate);
            events.AddRange(_dataChangesHandler.HandleRelates(toUpdate));
            events.AddRange(_dataChangesHandler.HandleUpdates(toUpdate));

            return events;
        }
    }
}