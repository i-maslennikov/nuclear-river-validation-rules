using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class FirmAddressCategoryAccessor : IStorageBasedDataObjectAccessor<FirmAddressCategory>, IDataChangesHandler<FirmAddressCategory>
    {
        private readonly IQuery _query;

        public FirmAddressCategoryAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<FirmAddressCategory> GetSource() => GetLevelThreeSource().Union(GetLevelOneSource());

        private IQueryable<FirmAddressCategory> GetLevelThreeSource() => _query
            .For<Erm::CategoryFirmAddress>()
            .Where(x => x.IsActive && !x.IsDeleted)
            .Select(x => new FirmAddressCategory
            {
                Id = x.Id,
                CategoryId = x.CategoryId,
                FirmAddressId = x.FirmAddressId,
            });

        private IQueryable<FirmAddressCategory> GetLevelOneSource()
            => from cfa in _query.For<Erm::CategoryFirmAddress>().Where(x => x.IsActive && !x.IsDeleted)
               from c3 in _query.For<Erm::Category>().Where(x => x.Id == cfa.CategoryId)
               from c2 in _query.For<Erm::Category>().Where(x => x.Id == c3.ParentId)
               where cfa.IsActive && !cfa.IsDeleted
               select new FirmAddressCategory
               {
                   Id = cfa.Id,
                   CategoryId = c2.ParentId,
                   FirmAddressId = cfa.FirmAddressId,
               };

        public FindSpecification<FirmAddressCategory> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return Specification<FirmAddressCategory>.Create(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<FirmAddressCategory> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmAddressCategory> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmAddressCategory> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<FirmAddressCategory> dataObjects)
        {
            var firmAddressIds = dataObjects.Select(x => x.FirmAddressId).ToArray();

            var firmIds =
                from firmAddress in _query.For<FirmAddress>().Where(x => firmAddressIds.Contains(x.Id))
                select firmAddress.FirmId;

            var orderIds =
                from firmAddress in _query.For<FirmAddress>().Where(x => firmAddressIds.Contains(x.Id))
                from order in _query.For<Order>().Where(x => x.FirmId == firmAddress.FirmId)
                select order.Id;

            return new EventCollectionHelper { { typeof(Firm), firmIds.Distinct() }, { typeof(Order), orderIds } };
        }
    }
}