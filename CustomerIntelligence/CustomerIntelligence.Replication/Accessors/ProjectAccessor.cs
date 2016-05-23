using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.CustomerIntelligence.Replication.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.Bit;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Replication.Accessors
{
    public sealed class ProjectAccessor : IStorageBasedDataObjectAccessor<Project>, IDataChangesHandler<Project>
    {
        private readonly IQuery _query;

        public ProjectAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Project> GetSource() => Specs.Map.Erm.ToFacts.Projects.Map(_query);

        public FindSpecification<Project> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Project>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Project> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Project), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Project> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Project), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Project> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Project), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Project> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<Project>(x => ids.Contains(x.Id));

            var complexIds = (from project in _query.For(specification)
                              join categoryOrganizationUnit in _query.For<CategoryOrganizationUnit>() on project.OrganizationUnitId equals categoryOrganizationUnit.OrganizationUnitId
                              select new StatisticsKey { ProjectId = project.Id, CategoryId = categoryOrganizationUnit.CategoryId })
                .ToArray();

            var territoryIds = (from project in _query.For(specification)
                                join territory in _query.For<Territory>() on project.OrganizationUnitId equals territory.OrganizationUnitId
                                select territory.Id)
                .ToArray();

            var firmIds = (from project in _query.For(specification)
                           join firm in _query.For<Firm>() on project.OrganizationUnitId equals firm.OrganizationUnitId
                           select firm.Id)
                .ToArray();

            return Enumerable.Empty<IEvent>()
                             .Concat(complexIds.Select(x => new RelatedDataObjectOutdatedEvent<StatisticsKey>(typeof(ProjectCategoryStatistics), x)))
                             .Concat(territoryIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Territory), x)))
                             .Concat(firmIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Firm), x)))
                             .ToArray();
        }
    }
}