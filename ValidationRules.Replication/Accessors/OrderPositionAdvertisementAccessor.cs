using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class OrderPositionAdvertisementAccessor : IStorageBasedDataObjectAccessor<OrderPositionAdvertisement>, IDataChangesHandler<OrderPositionAdvertisement>
    {
        private readonly IQuery _query;

        public OrderPositionAdvertisementAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<OrderPositionAdvertisement> GetSource() => _query
            .For<Erm::OrderPositionAdvertisement>()
            .Select(x => new OrderPositionAdvertisement
            {
                Id = x.Id,
                OrderPositionId = x.OrderPositionId,
                PositionId = x.PositionId,

                FirmAddressId = x.FirmAddressId,
                CategoryId = x.CategoryId,
                AdvertisementId = x.AdvertisementId,
                ThemeId = x.ThemeId,
            });

        public FindSpecification<OrderPositionAdvertisement> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<OrderPositionAdvertisement>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<OrderPositionAdvertisement> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<OrderPositionAdvertisement> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<OrderPositionAdvertisement> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<OrderPositionAdvertisement> dataObjects)
        {
            var orderPositionIds = dataObjects.Select(x => x.OrderPositionId).Distinct().ToArray();

            var orderIds =
                from op in _query.For<OrderPosition>().Where(x => orderPositionIds.Contains(x.Id))
                select op.OrderId;

            var firmIds =
                from order in _query.For<Order>().Where(x => orderIds.Contains(x.Id))
                select order.FirmId;

            return new EventCollectionHelper { { typeof(Order), orderIds.Distinct() }, { typeof(Firm), firmIds.Distinct() } };
        }
    }
}