using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Actors;
using NuClear.Replication.Core.API.DataObjects;
using NuClear.Replication.Core.API.Equality;
using NuClear.Storage.API.Readings;

namespace NuClear.CustomerIntelligence.Domain.Actors
{
    public sealed class ValueObjectActor<TDataObject> : DataObjectsActorBase<TDataObject> where TDataObject : class
    {
        private readonly IBulkRepository<TDataObject> _bulkRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;
        private readonly IDataChangesHandler<TDataObject> _dataChangesHandler;

        public ValueObjectActor(
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

        public ValueObjectActor(
           IQuery query,
           IBulkRepository<TDataObject> bulkRepository,
           IEqualityComparerFactory equalityComparerFactory,
           IStorageBasedDataObjectAccessor<TDataObject> storageBasedDataObjectAccessor)
            : this(query, bulkRepository, equalityComparerFactory, storageBasedDataObjectAccessor, new NullDataChangesHandler<TDataObject>())
        {
        }

        public override IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var events = new List<IEvent>();

            var changes = DetectChanges(commands, _equalityComparerFactory.CreateCompleteComparer<TDataObject>());

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