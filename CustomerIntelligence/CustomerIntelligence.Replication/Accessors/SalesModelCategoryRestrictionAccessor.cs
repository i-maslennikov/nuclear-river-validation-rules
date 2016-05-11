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
    public sealed class SalesModelCategoryRestrictionAccessor : IStorageBasedDataObjectAccessor<SalesModelCategoryRestriction>, IDataChangesHandler<SalesModelCategoryRestriction>
    {
        private readonly IQuery _query;

        public SalesModelCategoryRestrictionAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<SalesModelCategoryRestriction> GetSource() => Specs.Map.Erm.ToFacts.SalesModelCategoryRestrictions.Map(_query);

        public FindSpecification<SalesModelCategoryRestriction> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<SalesModelCategoryRestriction>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<SalesModelCategoryRestriction> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<SalesModelCategoryRestriction> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<SalesModelCategoryRestriction> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<SalesModelCategoryRestriction> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<SalesModelCategoryRestriction>(x => ids.Contains(x.Id));

            var projectIds = (from restriction in _query.For(specification)
                              select restriction.ProjectId)
                .Distinct()
                .ToArray();

            return projectIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Project), x))
                             .ToArray();
        }
    }
}