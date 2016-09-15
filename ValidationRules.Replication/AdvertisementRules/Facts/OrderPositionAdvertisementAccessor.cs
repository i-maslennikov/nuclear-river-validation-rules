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
    public sealed class OrderPositionAdvertisementAccessor : IStorageBasedDataObjectAccessor<OrderPositionAdvertisement>, IDataChangesHandler<OrderPositionAdvertisement>
    {
        private readonly IQuery _query;

        public OrderPositionAdvertisementAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<OrderPositionAdvertisement> GetSource() => _query
            .For(Specs.Find.Erm.OrderPositionAdvertisements())
            .Select(x => new OrderPositionAdvertisement
            {
                Id = x.Id,
                OrderPositionId = x.OrderPositionId,
                PositionId = x.PositionId,
                AdvertisementId = x.AdvertisementId,
            });

        public FindSpecification<OrderPositionAdvertisement> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<OrderPositionAdvertisement>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<OrderPositionAdvertisement> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<OrderPositionAdvertisement> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<OrderPositionAdvertisement> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<OrderPositionAdvertisement> dataObjects)
        {
            var dataObjectIds = dataObjects.Select(x => x.Id).ToArray();

            var orderIds =
                from opa in _query.For<OrderPositionAdvertisement>().Where(x => dataObjectIds.Contains(x.Id))
                join op in _query.For<OrderPosition>() on opa.OrderPositionId equals op.Id
                join order in _query.For<Order>() on op.OrderId equals order.Id
                select order.Id;

            return new EventCollectionHelper { { typeof(Order), orderIds.Distinct() } }.ToArray();
        }
    }
}