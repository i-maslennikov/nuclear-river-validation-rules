using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Specs;
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

        public IQueryable<OrderPositionAdvertisement> GetSource()
            => from opa in _query.For<Erm::OrderPositionAdvertisement>()
               select new OrderPositionAdvertisement
                   {
                       Id = opa.Id,
                       OrderPositionId = opa.OrderPositionId,
                       PositionId = opa.PositionId,

                       FirmAddressId = opa.FirmAddressId,
                       CategoryId = opa.CategoryId,
                       AdvertisementId = opa.AdvertisementId,
                       ThemeId = opa.ThemeId,
                   };

        public FindSpecification<OrderPositionAdvertisement> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToList();
            return SpecificationFactory<OrderPositionAdvertisement>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<OrderPositionAdvertisement> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<OrderPositionAdvertisement> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<OrderPositionAdvertisement> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<OrderPositionAdvertisement> dataObjects)
        {
            var orderPositionIds = dataObjects.Select(x => x.OrderPositionId).Distinct().ToList();

            var orderIds =
                from op in _query.For<OrderPosition>().Where(x => orderPositionIds.Contains(x.Id))
                select op.OrderId;

            var firmIds =
                from order in _query.For<Order>().Where(x => orderIds.Contains(x.Id))
                select order.FirmId;

            return new EventCollectionHelper<OrderPositionAdvertisement> { { typeof(Order), orderIds }, { typeof(Firm), firmIds } };
        }
    }
}