using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.API.Equality;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.API
{
    public sealed class CreateDataObjectsActor<TDataObject> : DataObjectsActorBase<TDataObject>
        where TDataObject : class
    {
        private readonly IBulkRepository<TDataObject> _bulkRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;
        private readonly IDataChangesHandler<TDataObject> _dataChangesHandler;

        public CreateDataObjectsActor(
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

        public CreateDataObjectsActor(
            IQuery query,
            IBulkRepository<TDataObject> bulkRepository,
            IEqualityComparerFactory equalityComparerFactory,
            IStorageBasedDataObjectAccessor<TDataObject> storageBasedDataObjectAccessor)
            : this(query, bulkRepository, equalityComparerFactory, storageBasedDataObjectAccessor, new NullDataChangesHandler<TDataObject>())
        {
        }

        public override IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var changes = DetectChanges(commands, _equalityComparerFactory.CreateIdentityComparer<TDataObject>());

            var toCreate = changes.Difference.ToArray();

            _bulkRepository.Create(toCreate);
            return _dataChangesHandler.HandleCreates(toCreate);
        }
    }
}