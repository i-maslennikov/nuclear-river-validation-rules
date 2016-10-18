using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.ThemeRules.Facts;

namespace NuClear.ValidationRules.Replication.ThemeRules.Facts
{
    public sealed class ThemeOrganizationUnitAccessor : IStorageBasedDataObjectAccessor<ThemeOrganizationUnit>, IDataChangesHandler<ThemeOrganizationUnit>
    {
        private readonly IQuery _query;

        public ThemeOrganizationUnitAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<ThemeOrganizationUnit> GetSource()
            => _query.For(Specs.Find.Erm.ThemeOrganizationUnit()).Select(x => new ThemeOrganizationUnit
            {
                Id = x.Id,
                ThemeId = x.ThemeId,
                OrganizationUnitId = x.OrganizationUnitId,
            });

        public FindSpecification<ThemeOrganizationUnit> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<ThemeOrganizationUnit>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<ThemeOrganizationUnit> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<ThemeOrganizationUnit> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<ThemeOrganizationUnit> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<ThemeOrganizationUnit> dataObjects)
        {
            var organizationUnitIds = dataObjects.Select(x => x.OrganizationUnitId).Distinct().ToArray();

            var projectIds =
                from project in _query.For<Project>().Where(x => organizationUnitIds.Contains(x.OrganizationUnitId))
                select project.Id;

            return new EventCollectionHelper { { typeof(Project), projectIds.Distinct() } }.ToArray();
        }
    }
}