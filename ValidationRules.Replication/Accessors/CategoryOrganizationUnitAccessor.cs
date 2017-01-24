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
    public sealed class CategoryOrganizationUnitAccessor : IStorageBasedDataObjectAccessor<CategoryOrganizationUnit>, IDataChangesHandler<CategoryOrganizationUnit>
    {
        private readonly IQuery _query;

        public CategoryOrganizationUnitAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<CategoryOrganizationUnit> GetSource() => _query
            .For<Erm::CategoryOrganizationUnit>()
            .Where(x => x.IsActive && !x.IsDeleted)
            .Select(x => new CategoryOrganizationUnit
                {
                    Id = x.Id,
                    CategoryId = x.CategoryId,
                    OrganizationUnitId = x.OrganizationUnitId
                });

        public FindSpecification<CategoryOrganizationUnit> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToList();
            return SpecificationFactory<CategoryOrganizationUnit>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<CategoryOrganizationUnit> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<CategoryOrganizationUnit> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<CategoryOrganizationUnit> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<CategoryOrganizationUnit> dataObjects)
        {
            var ids = dataObjects.Select(x => x.OrganizationUnitId).Distinct();

            var projectIds =
                from project in _query.For<Project>().Where(x => ids.Contains(x.OrganizationUnitId))
                select project.Id;

            return new EventCollectionHelper<CategoryOrganizationUnit> { { typeof(Project), projectIds } };
        }
    }
}