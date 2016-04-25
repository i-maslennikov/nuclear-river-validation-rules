using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Events;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Accessors
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
            => new FindSpecification<Territory>(x => commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).Contains(x.Id));

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Territory> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Territory), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Territory> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Territory), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Territory> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Territory), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Territory> dataObjects) => Array.Empty<IEvent>();
    }
}