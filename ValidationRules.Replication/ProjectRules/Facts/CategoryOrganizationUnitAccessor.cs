using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Facts;

namespace NuClear.ValidationRules.Replication.ProjectRules.Facts
{
    public sealed class CategoryOrganizationUnitAccessor : IStorageBasedDataObjectAccessor<CategoryOrganizationUnit>, IDataChangesHandler<CategoryOrganizationUnit>
    {
        private readonly IQuery _query;

        public CategoryOrganizationUnitAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<CategoryOrganizationUnit> GetSource()
            => from x in _query.For<Storage.Model.Erm.CategoryOrganizationUnit>()
               where x.IsActive && !x.IsDeleted
               select new CategoryOrganizationUnit { Id = x.Id, CategoryId = x.CategoryId, OrganizationUnitId = x.OrganizationUnitId };

        public FindSpecification<CategoryOrganizationUnit> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<CategoryOrganizationUnit>(x => ids.Contains(x.Id));
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

            return new EventCollectionHelper { { typeof(Project), projectIds.Distinct() } };
        }
    }
}