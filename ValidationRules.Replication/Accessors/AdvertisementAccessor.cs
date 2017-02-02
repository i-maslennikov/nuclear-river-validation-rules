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
    public sealed class AdvertisementAccessor : IStorageBasedDataObjectAccessor<Advertisement>, IDataChangesHandler<Advertisement>
    {
        private readonly IQuery _query;

        public AdvertisementAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Advertisement> GetSource() => _query
            .For(Specs.Find.Erm.Advertisement)
            .Select(x => new Advertisement
            {
                Id = x.Id,
                FirmId = x.FirmId,
                AdvertisementTemplateId = x.AdvertisementTemplateId,
                IsSelectedToWhiteList = x.IsSelectedToWhiteList,
                IsDeleted = x.IsDeleted
            });

        public FindSpecification<Advertisement> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToList();
            return SpecificationFactory<Advertisement>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Advertisement> dataObjects) => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Advertisement), x.Id)).ToList();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Advertisement> dataObjects) => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Advertisement), x.Id)).ToList();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Advertisement> dataObjects) => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Advertisement), x.Id)).ToList();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Advertisement> dataObjects)
        {
            var dataObjectIds = dataObjects.Select(x => x.Id).ToList();

            var orderIds =
                from opa in _query.For<OrderPositionAdvertisement>().Where(x => dataObjectIds.Contains((long)x.AdvertisementId))
                join op in _query.For<OrderPosition>() on opa.OrderPositionId equals op.Id
                join order in _query.For<Order>() on op.OrderId equals order.Id
                select order.Id;

            var firmIds =
                from advertisement in _query.For<Advertisement>().Where(x => dataObjectIds.Contains(x.Id))
                where advertisement.FirmId != null
                select advertisement.FirmId.Value;

            return new EventCollectionHelper<Advertisement> { { typeof(Order), orderIds }, {typeof(Firm), firmIds } };
        }
    }
}