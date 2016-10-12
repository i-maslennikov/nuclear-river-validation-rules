using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Facts;

namespace NuClear.ValidationRules.Replication.ProjectRules.Facts
{
    public sealed class FirmAddressAccessor : IStorageBasedDataObjectAccessor<FirmAddress>, IDataChangesHandler<FirmAddress>
    {
        private readonly IQuery _query;

        public FirmAddressAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<FirmAddress> GetSource()
            => from x in _query.For(Specs.Find.Erm.FirmAddresses())
               select new FirmAddress { Id = x.Id, Name = x.Address, IsLocatedOnTheMap = x.IsLocatedOnTheMap };

        public FindSpecification<FirmAddress> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<FirmAddress>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<FirmAddress> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(FirmAddress), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmAddress> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(FirmAddress), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmAddress> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(FirmAddress), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<FirmAddress> dataObjects)
        {
            var firmAddressIds = dataObjects.Select(x => x.Id);

            var orderIds =
                from opa in _query.For<OrderPositionAdvertisement>().Where(x => x.FirmAddressId.HasValue && firmAddressIds.Contains(x.FirmAddressId.Value))
                from op in _query.For<OrderPosition>().Where(x => x.Id == opa.OrderPositionId)
                select op.OrderId;

            return new EventCollectionHelper { { typeof(Order), orderIds.Distinct() } };
        }
    }
}