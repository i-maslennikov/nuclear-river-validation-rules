using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Facts
{
    public sealed class FirmAddressAccessor : IStorageBasedDataObjectAccessor<FirmAddress>, IDataChangesHandler<FirmAddress>
    {
        private readonly IQuery _query;

        public FirmAddressAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<FirmAddress> GetSource()
            => from address in _query.For<Erm::FirmAddress>()
               select new FirmAddress
                   {
                       Id = address.Id,
                       FirmId = address.FirmId,
                       IsClosedForAscertainment = address.ClosedForAscertainment,
                       IsActive = address.IsActive,
                       IsDeleted = address.IsDeleted,
                       Name = address.Address,
                   };

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
            => Array.Empty<IEvent>();
    }
}
