using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.Actors
{
    public abstract class EntityActorBase<TDataObject> : IEntityActor
        where TDataObject : class
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<TDataObject> _bulkRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;
        private readonly IStorageBasedDataObjectAccessor<TDataObject> _storageBasedDataObjectAccessor;

        protected EntityActorBase(
            IQuery query,
            IBulkRepository<TDataObject> bulkRepository,
            IEqualityComparerFactory equalityComparerFactory,
            IStorageBasedDataObjectAccessor<TDataObject> storageBasedDataObjectAccessor)
        {
            _query = query;
            _bulkRepository = bulkRepository;
            _equalityComparerFactory = equalityComparerFactory;
            _storageBasedDataObjectAccessor = storageBasedDataObjectAccessor;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            if (!commands.Any())
            {
                return Array.Empty<IEvent>();
            }

            var events = new List<IEvent>();

            IActor actor = new CreateDataObjectsActor<TDataObject>(_query, _bulkRepository, _equalityComparerFactory, _storageBasedDataObjectAccessor);
            events.AddRange(actor.ExecuteCommands(commands));

            actor = new SyncDataObjectsActor<TDataObject>(_query, _bulkRepository, _equalityComparerFactory, _storageBasedDataObjectAccessor);
            events.AddRange(actor.ExecuteCommands(commands));

            actor = new DeleteDataObjectsActor<TDataObject>(_query, _bulkRepository, _equalityComparerFactory, _storageBasedDataObjectAccessor);
            events.AddRange(actor.ExecuteCommands(commands));

            return events;
        }

        public abstract IReadOnlyCollection<IActor> GetValueObjectActors();
    }
}