using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.Commands;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.Actors
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
            var commandsToExecute = commands.OfType<ISyncDataObjectCommand>()
                                            .Where(x => x.DataObjectType == typeof(TDataObject))
                                            .Distinct()
                                            .ToArray();

            if (!commandsToExecute.Any())
            {
                return Array.Empty<IEvent>();
            }

            var events = new List<IEvent>();

            var changes = DetectChanges(commandsToExecute,
                                        _equalityComparerFactory.CreateIdentityComparer<TDataObject>(),
                                        _equalityComparerFactory.CreateCompleteComparer<TDataObject>());

            var toDelete = changes.Complement.ToArray();

            events.AddRange(_dataChangesHandler.HandleRelates(toDelete));
            events.AddRange(_dataChangesHandler.HandleDeletes(toDelete));
            _bulkRepository.Delete(toDelete);

            var toCreate = changes.Difference.ToArray();

            _bulkRepository.Create(toCreate);
            events.AddRange(_dataChangesHandler.HandleCreates(toCreate));
            events.AddRange(_dataChangesHandler.HandleRelates(toCreate));


            var toUpdate = changes.Intersection.ToArray();

            events.AddRange(_dataChangesHandler.HandleRelates(toUpdate));
            _bulkRepository.Update(toUpdate);
            events.AddRange(_dataChangesHandler.HandleRelates(toUpdate));
            events.AddRange(_dataChangesHandler.HandleUpdates(toUpdate));

            return events;
        }
    }
}