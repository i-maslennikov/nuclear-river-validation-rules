using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Facts
{
    public sealed class DealAccessor : IStorageBasedDataObjectAccessor<Deal>, IDataChangesHandler<Deal>
    {
        private readonly IQuery _query;

        public DealAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Deal> GetSource()
            => _query.For<Storage.Model.Erm.Deal>()
                     .Where(x => x.IsActive && !x.IsDeleted)
                     .Select(x => new Deal { Id = x.Id });

        public FindSpecification<Deal> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Deal>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Deal> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Deal> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Deal> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Deal> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id);

            var orderIds =
                from order in _query.For<Order>().Where(x => ids.Contains(x.DealId.Value))
                select order.Id;

            return new EventCollectionHelper { { typeof(Order), orderIds } };
        }
    }
}