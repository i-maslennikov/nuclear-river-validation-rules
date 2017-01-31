using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Specs;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class FirmAddressAccessor : IStorageBasedDataObjectAccessor<FirmAddress>, IDataChangesHandler<FirmAddress>
    {
        private readonly IQuery _query;

        public FirmAddressAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<FirmAddress> GetSource() => _query
            .For<Erm::FirmAddress>()
            .Select(x => new FirmAddress
            {
                Id = x.Id,
                FirmId = x.FirmId,

                IsLocatedOnTheMap = x.IsLocatedOnTheMap,

                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted,
                IsClosedForAscertainment = x.ClosedForAscertainment,
            });

        public FindSpecification<FirmAddress> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToList();
            return SpecificationFactory<FirmAddress>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<FirmAddress> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(FirmAddress), x.Id)).ToList();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmAddress> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(FirmAddress), x.Id)).ToList();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmAddress> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(FirmAddress), x.Id)).ToList();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<FirmAddress> dataObjects)
        {
            var firmIds = dataObjects.Select(x => x.FirmId);

            var orderIds =
                from order in _query.For<Order>().Where(x => firmIds.Contains(x.FirmId))
                select order.Id;

            return new EventCollectionHelper<FirmAddress> { { typeof(Firm), firmIds }, { typeof(Order), orderIds } };
        }
    }
}