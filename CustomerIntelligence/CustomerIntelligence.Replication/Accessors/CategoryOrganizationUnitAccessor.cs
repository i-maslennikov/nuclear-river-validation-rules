using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.CustomerIntelligence.Replication.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Replication.Accessors
{
    public sealed class CategoryOrganizationUnitAccessor : IStorageBasedDataObjectAccessor<CategoryOrganizationUnit>, IDataChangesHandler<CategoryOrganizationUnit>
    {
        private readonly IQuery _query;

        public CategoryOrganizationUnitAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<CategoryOrganizationUnit> GetSource() => Specs.Map.Erm.ToFacts.CategoryOrganizationUnits.Map(_query);

        public FindSpecification<CategoryOrganizationUnit> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<CategoryOrganizationUnit>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<CategoryOrganizationUnit> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<CategoryOrganizationUnit> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<CategoryOrganizationUnit> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<CategoryOrganizationUnit> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<CategoryOrganizationUnit>(x => ids.Contains(x.Id));

            IEnumerable<IEvent> events = Specs.Map.Facts.ToProjectAggregate.ByCategoryOrganizationUnit(specification)
                                              .Map(_query)
                                              .Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Project), x));

            events = events.Concat(Specs.Map.Facts.ToFirmAggregate.ByCategoryOrganizationUnit(specification)
                                        .Map(_query)
                                        .Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Firm), x)));

            events = events.Concat(Specs.Map.Facts.ToClientAggregate.ByCategoryOrganizationUnit(specification)
                                        .Map(_query)
                                        .Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Client), x)));
            return events.ToArray();
        }
    }
}