using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Facts
{
    public sealed class AdvertisementAccessor : IStorageBasedDataObjectAccessor<Advertisement>, IDataChangesHandler<Advertisement>
    {
        private readonly IQuery _query;

        public AdvertisementAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Advertisement> GetSource() => _query
            .For<Storage.Model.Erm.Advertisement>()
            .Select(x => new Advertisement
            {
                Id = x.Id,
                FirmId = x.FirmId,
                AdvertisementTemplateId = x.AdvertisementTemplateId,
                Name = x.Name,
                IsSelectedToWhiteList = x.IsSelectedToWhiteList,
                IsDeleted = x.IsDeleted
            });

        public FindSpecification<Advertisement> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Advertisement>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Advertisement> dataObjects) => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Advertisement), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Advertisement> dataObjects) => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Advertisement), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Advertisement> dataObjects) => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Advertisement), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Advertisement> dataObjects)
        {
            var dataObjectIds = dataObjects.Select(x => x.Id).ToArray();

            var orderIds =
                from advertisement in _query.For<Advertisement>().Where(x => dataObjectIds.Contains(x.Id))
                join opa in _query.For<OrderPositionAdvertisement>() on advertisement.Id equals opa.AdvertisementId
                join op in _query.For<OrderPosition>() on opa.OrderPositionId equals op.Id
                join order in _query.For<Order>() on op.OrderId equals order.Id
                select order.Id;

            return new EventCollectionHelper { { typeof(Order), orderIds.Distinct() }, {typeof(Advertisement), dataObjectIds } }.ToArray();
        }
    }
}