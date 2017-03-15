using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Specs;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class OrderItemAccessor : IStorageBasedDataObjectAccessor<OrderItem>, IDataChangesHandler<OrderItem>
    {
        private readonly IQuery _query;

        public OrderItemAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<OrderItem> GetSource()
        {
            // join тут можно использовать:
            //  PricePosition не меняет PositionId, поэтому его изменения ни на что не влияют
            //  OrderPositionAdvertisement практически честный value-object сущности OrderPosition - когда происходят изменения в OrderPositionAdvertisement всегда логируеется изменение OrderPosition
            //  Order может быть изменён независимо, но он влияет на наличие/отсутствие OrderItem, а не на его содержимое (подобную оптимизацию мы использовали в river-ci)

            var opas =
                from order in _query.For<Erm::Order>().Where(Specs.Find.Erm.Order)
                from orderPosition in _query.For<Erm::OrderPosition>().Where(Specs.Find.Erm.OrderPosition).Where(x => x.OrderId == order.Id)
                from pricePosition in _query.For<Erm::PricePosition>().Where(x => x.Id == orderPosition.PricePositionId)
                from opa in _query.For<Erm::OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id)
                select new OrderItem
                    {
                        OrderId = orderPosition.OrderId,
                        OrderPositionId = orderPosition.Id,
                        PricePositionId = null,
                        PackagePositionId = pricePosition.PositionId,
                        ItemPositionId = opa.PositionId,

                        FirmAddressId = opa.FirmAddressId,
                        CategoryId = opa.CategoryId,
                    };

            var pkgs =
                from order in _query.For<Erm::Order>().Where(Specs.Find.Erm.Order)
                from orderPosition in _query.For<Erm::OrderPosition>().Where(Specs.Find.Erm.OrderPosition).Where(x => x.OrderId == order.Id)
                from pricePosition in _query.For<Erm::PricePosition>().Where(x => x.Id == orderPosition.PricePositionId)
                from opa in _query.For<Erm::OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id)
                select new OrderItem
                    {
                        OrderId = orderPosition.OrderId,
                        OrderPositionId = orderPosition.Id,
                        PricePositionId = orderPosition.PricePositionId,
                        PackagePositionId = pricePosition.PositionId,
                        ItemPositionId = pricePosition.PositionId,

                        FirmAddressId = opa.FirmAddressId,
                        CategoryId = opa.CategoryId,
                    };

            return opas.Union(pkgs);
        }

        public FindSpecification<OrderItem> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToList();
            return SpecificationFactory<OrderItem>.Contains(x => x.OrderPositionId, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<OrderItem> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<OrderItem> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<OrderItem> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<OrderItem> dataObjects)
        {
            var orderIds = dataObjects.Select(x => x.OrderId);

            var firmIds =
                from order in _query.For<Order>().Where(x => orderIds.Contains(x.Id))
                select order.FirmId;

            return new EventCollectionHelper<OrderItem> { { typeof(Firm), firmIds } };
        }
    }
}