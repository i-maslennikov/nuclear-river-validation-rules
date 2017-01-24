using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Specs;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
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
            .For<Erm::FirmContact>()
            .Where(x => x.FirmAddressId != null)
            .Where(x => x.ContactType == Website)
            .Select(x => new FirmAddressWebsite
            {
                Id = x.Id,
                FirmAddressId = x.FirmAddressId.Value,
                Website = x.Contact,
            });

        public FindSpecification<FirmAddressWebsite> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToList();
            return SpecificationFactory<FirmAddressWebsite>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<FirmAddressWebsite> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmAddressWebsite> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmAddressWebsite> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<FirmAddressWebsite> dataObjects)
        {
            var firmAddressIds = dataObjects.Select(x => x.FirmAddressId).ToList();

            var firmIds =
                from firmAddress in _query.For<FirmAddress>().Where(x => firmAddressIds.Contains(x.Id))
                select firmAddress.FirmId;

            return new EventCollectionHelper<FirmAddressWebsite> { { typeof(Firm), firmIds } };
        }
    }
}