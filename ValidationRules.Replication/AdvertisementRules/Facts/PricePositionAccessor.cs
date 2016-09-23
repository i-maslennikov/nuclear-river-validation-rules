using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Facts
{
    public sealed class PricePositionAccessor : IStorageBasedDataObjectAccessor<PricePosition>, IDataChangesHandler<PricePosition>
    {
        private readonly IQuery _query;

        public PricePositionAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<PricePosition> GetSource() => _query
            .For(Specs.Find.Erm.PricePositions())
            .Select(x => new PricePosition
            {
                Id = x.Id,
                PositionId = x.PositionId,
            });

        public FindSpecification<PricePosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<PricePosition>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<PricePosition> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<PricePosition> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<PricePosition> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<PricePosition> dataObjects)
        {
            var dataObjectIds = dataObjects.Select(x => x.Id).ToArray();

            var orderIds =
                from op in _query.For<OrderPosition>().Where(x => dataObjectIds.Contains(x.PricePositionId))
                join order in _query.For<Order>() on op.OrderId equals order.Id
                select order.Id;

            return new EventCollectionHelper { { typeof(Order), orderIds.Distinct() } }.ToArray();
        }
    }
}