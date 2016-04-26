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

            IEnumerable<IEvent> events = Specs.Map.Facts.ToStatistics.ByProject(specification)
                                              .Map(_query)
                                              .Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(ProjectCategoryStatistics), x));

            events = events.Concat(Specs.Map.Facts.ToTerritoryAggregate.ByProject(specification)
                                        .Map(_query)
                                        .Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Territory), x)));

            events = events.Concat(Specs.Map.Facts.ToFirmAggregate.ByProject(specification)
                                        .Map(_query)
                                        .Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Firm), x)));
            return events.ToArray();
        }
    }
}