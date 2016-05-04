using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.Commands;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.Actors
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
            var commandsToExecute = commands.OfType<IReplaceValueObjectCommand>().Distinct().ToArray();
            if (!commandsToExecute.Any())
            {
                return Array.Empty<IEvent>();
            }

            var events = new List<IEvent>();

            var changes = DetectChanges(commandsToExecute, _equalityComparerFactory.CreateCompleteComparer<TDataObject>());

            var toDelete = changes.Complement.ToArray();

            events.AddRange(_dataChangesHandler.HandleRelates(toDelete));
            events.AddRange(_dataChangesHandler.HandleDeletes(toDelete));
            _bulkRepository.Delete(toDelete);
            events.AddRange(_dataChangesHandler.HandleRelates(toDelete));

            var toCreate = changes.Difference.ToArray();

            events.AddRange(_dataChangesHandler.HandleRelates(toCreate));
            _bulkRepository.Create(toCreate);
            events.AddRange(_dataChangesHandler.HandleRelates(toCreate));
            events.AddRange(_dataChangesHandler.HandleCreates(toCreate));

            return events;
        }
    }
}