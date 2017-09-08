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
    public sealed class FirmAddressAccessor : IStorageBasedDataObjectAccessor<FirmAddress>, IDataChangesHandler<FirmAddress>
    {
        private readonly IQuery _query;

        public FirmAddressAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<FirmAddress> GetSource() => _query
            .For(Specs.Find.Erm.FirmAddress)
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
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmAddress> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmAddress> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<FirmAddress> dataObjects)
        {
            var firmIds = dataObjects.Select(x => x.FirmId);
            var firmAddressIds = dataObjects.Select(x => x.Id);

            var orderIdsByFirm =
                from order in _query.For<Order>().Where(x => firmIds.Contains(x.FirmId))
                select order.Id;

            var orderIdsByUsage =
                from opa in _query.For<OrderPositionAdvertisement>().Where(x => firmAddressIds.Contains(x.FirmAddressId.Value))
                from op in _query.For<OrderPosition>().Where(x => x.Id == opa.OrderPositionId)
                select op.OrderId;

            return new EventCollectionHelper<FirmAddress> { { typeof(Firm), firmIds }, { typeof(Order), orderIdsByFirm.Union(orderIdsByUsage) } };
        }
    }
}