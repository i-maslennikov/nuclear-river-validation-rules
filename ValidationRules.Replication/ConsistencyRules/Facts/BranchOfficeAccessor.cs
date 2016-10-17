using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Facts
{
    public sealed class BranchOfficeAccessor : IStorageBasedDataObjectAccessor<BranchOffice>, IDataChangesHandler<BranchOffice>
    {
        private readonly IQuery _query;

        public BranchOfficeAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<BranchOffice> GetSource()
            => _query.For<Storage.Model.Erm.BranchOffice>()
                     .Where(x => x.IsActive && !x.IsDeleted)
                     .Select(x => new BranchOffice { Id = x.Id });

        public FindSpecification<BranchOffice> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<BranchOffice>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<BranchOffice> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<BranchOffice> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<BranchOffice> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<BranchOffice> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id);

            var orderIds =
                from order in _query.For<Order>()
                from bo in _query.For<BranchOfficeOrganizationUnit>().Where(x => ids.Contains(x.BranchOfficeId))
                select order.Id;

            return new EventCollectionHelper { { typeof(Order), orderIds } };
        }
    }
}