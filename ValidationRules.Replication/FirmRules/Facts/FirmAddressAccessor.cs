using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.FirmRules.Facts;

namespace NuClear.ValidationRules.Replication.FirmRules.Facts
{
    public sealed class FirmAddressAccessor : IStorageBasedDataObjectAccessor<FirmAddress>, IDataChangesHandler<FirmAddress>
    {
        private readonly IQuery _query;

        public FirmAddressAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<FirmAddress> GetSource()
            => from firmAddress in _query.For(Specs.Find.Erm.FirmAddresses())
               select new FirmAddress
                   {
                       Id = firmAddress.Id,
                       FirmId = firmAddress.FirmId
                   };

        public FindSpecification<FirmAddress> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<FirmAddress>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<FirmAddress> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmAddress> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmAddress> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<FirmAddress> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();

            var firmIds =
                from firmAddress in _query.For<FirmAddress>().Where(x => ids.Contains(x.Id))
                select firmAddress.FirmId;

            return new EventCollectionHelper { { typeof(Firm), firmIds.Distinct() } };
        }
    }
}