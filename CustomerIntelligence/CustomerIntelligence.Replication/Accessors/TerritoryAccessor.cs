using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.CustomerIntelligence.Replication.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Replication.Accessors
{
    public sealed class TerritoryAccessor : IStorageBasedDataObjectAccessor<Territory>, IDataChangesHandler<Territory>
    {
        private readonly IQuery _query;

        public TerritoryAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Territory> GetSource() => Specs.Map.Erm.ToFacts.Territories.Map(_query);

        public FindSpecification<Territory> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Territory>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Territory> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Territory), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Territory> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Territory), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Territory> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Territory), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Territory> dataObjects) => Array.Empty<IEvent>();
    }
}