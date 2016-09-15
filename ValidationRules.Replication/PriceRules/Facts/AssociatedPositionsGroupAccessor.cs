using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.PriceRules.Facts;

namespace NuClear.ValidationRules.Replication.PriceRules.Facts
{
    public sealed class AssociatedPositionsGroupAccessor : IStorageBasedDataObjectAccessor<AssociatedPositionsGroup>, IDataChangesHandler<AssociatedPositionsGroup>
    {
        private readonly IQuery _query;

        public AssociatedPositionsGroupAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<AssociatedPositionsGroup> GetSource() => Specs.Map.Erm.ToFacts.AssociatedPositionsGroup.Map(_query);

        public FindSpecification<AssociatedPositionsGroup> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<AssociatedPositionsGroup>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<AssociatedPositionsGroup> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<AssociatedPositionsGroup> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<AssociatedPositionsGroup> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<AssociatedPositionsGroup> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();

            var priceIds = from associatedPositionsGroup in _query.For<AssociatedPositionsGroup>().Where(x => ids.Contains(x.Id))
                           from pricePosition in _query.For<PricePosition>().Where(x => x.Id == associatedPositionsGroup.PricePositionId)
                           select pricePosition.PriceId;

            priceIds = priceIds.Distinct();

            var orderIds = from associatedPositionsGroup in _query.For<AssociatedPositionsGroup>().Where(x => ids.Contains(x.Id))
                           from orderPosition in _query.For<OrderPosition>().Where(x => x.PricePositionId == associatedPositionsGroup.PricePositionId)
                           select orderPosition.OrderId;

            orderIds = orderIds.Distinct();

            return priceIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Price), x))
                           .Union(orderIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Order), x)))
                           .ToArray();
        }
    }
}