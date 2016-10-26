using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Facts
{
    public sealed class FirmAddressWebsiteAccessor : IStorageBasedDataObjectAccessor<FirmAddressWebsite>, IDataChangesHandler<FirmAddressWebsite>
    {
        private const int Website = 4;

        private readonly IQuery _query;

        public FirmAddressWebsiteAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<FirmAddressWebsite> GetSource() => _query
            .For(Specs.Find.Erm.FirmContact())
            .Where(x => x.ContactType == Website)
            .Select(x => new FirmAddressWebsite
            {
                Id = x.Id,
                FirmAddressId = x.FirmAddressId.Value,
                Website = x.Contact,
            });

        public FindSpecification<FirmAddressWebsite> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<FirmAddressWebsite>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<FirmAddressWebsite> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmAddressWebsite> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmAddressWebsite> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<FirmAddressWebsite> dataObjects)
        {
            var firmAddressIds = dataObjects.Select(x => x.FirmAddressId).ToArray();

            var firmIds =
                from firmAddress in _query.For<FirmAddress>().Where(x => firmAddressIds.Contains(x.Id))
                select firmAddress.FirmId;

            return new EventCollectionHelper { { typeof(Order), firmIds.Distinct() } }.ToArray();
        }
    }
}