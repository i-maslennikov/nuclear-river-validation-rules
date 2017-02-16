using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Specs;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class PositionAccessor : IStorageBasedDataObjectAccessor<Position>, IDataChangesHandler<Position>
    {
        private readonly IQuery _query;

        public PositionAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Position> GetSource() => _query
            .For(Specs.Find.Erm.Position)
            .Select(x => new Position
            {
                Id = x.Id,

                AdvertisementTemplateId = x.AdvertisementTemplateId,
                BindingObjectType = x.BindingObjectTypeEnum,
                SalesModel = x.SalesModel,
                PositionsGroup = x.PositionsGroup,

                IsCompositionOptional = x.IsCompositionOptional,
                IsComposite = x.IsComposite,
                IsControlledByAmount = x.IsControlledByAmount,

                CategoryCode = x.CategoryCode,
                Platform = x.Platform,
                IsDeleted = x.IsDeleted,
            });

        public FindSpecification<Position> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToList();
            return SpecificationFactory<Position>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Position> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Position), x.Id)).ToList();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Position> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Position), x.Id)).ToList();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Position> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Position), x.Id)).ToList();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Position> dataObjects)
        {
            var positionIds = dataObjects.Select(x => x.Id).ToList();

            var orderIdsFromOpa =
                from pricePosition in _query.For<OrderPositionAdvertisement>().Where(x => positionIds.Contains(x.PositionId))
                from orderPosition in _query.For<OrderPosition>().Where(x => x.Id == pricePosition.OrderPositionId)
                select orderPosition.OrderId;

            var orderIdsFromPricePosition =
                  from pricePosition in _query.For<PricePosition>().Where(x => positionIds.Contains(x.PositionId))
                  from orderPosition in _query.For<OrderPosition>().Where(x => x.PricePositionId == pricePosition.Id)
                  select orderPosition.OrderId;

            var orderIds = orderIdsFromOpa.Union(orderIdsFromPricePosition);

            var firmIds =
                from order in _query.For<Order>().Where(x => orderIds.Contains(x.Id))
                select order.FirmId;

            var priceIds = from pricePosition in _query.For<PricePosition>().Where(x => positionIds.Contains(x.PositionId))
                           select pricePosition.PriceId;

            return new EventCollectionHelper<Position> { { typeof(Order), orderIds }, { typeof(Firm), firmIds }, { typeof(Price), priceIds } };
        }
    }
}