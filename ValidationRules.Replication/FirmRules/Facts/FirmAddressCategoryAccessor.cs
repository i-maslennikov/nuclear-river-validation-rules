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
    public sealed class FirmAddressCategoryAccessor : IStorageBasedDataObjectAccessor<FirmAddressCategory>, IDataChangesHandler<FirmAddressCategory>
    {
        private readonly IQuery _query;

        public FirmAddressCategoryAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<FirmAddressCategory> GetSource()
            => from categoryFirmAddress in _query.For(Specs.Find.Erm.CategoryFirmAddresses())
               select new FirmAddressCategory
                   {
                       Id = categoryFirmAddress.Id,
                       CategoryId = categoryFirmAddress.CategoryId,
                       FirmAddressId = categoryFirmAddress.FirmAddressId,
                   };

        public FindSpecification<FirmAddressCategory> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<FirmAddressCategory>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<FirmAddressCategory> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmAddressCategory> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmAddressCategory> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<FirmAddressCategory> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();

            var firmIds =
                from firmAddressCategory in _query.For<FirmAddressCategory>().Where(x => ids.Contains(x.Id))
                from firmAddress in _query.For<FirmAddress>().Where(x => x.Id == firmAddressCategory.FirmAddressId)
                select firmAddress.FirmId;

            return new EventCollectionHelper { { typeof(Firm), firmIds.Distinct() } };
        }
    }
}