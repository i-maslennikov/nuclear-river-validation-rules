using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Events;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
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
            => new FindSpecification<SalesModelCategoryRestriction>(x => commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).Contains(x.Id));

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<SalesModelCategoryRestriction> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(SalesModelCategoryRestriction), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<SalesModelCategoryRestriction> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(SalesModelCategoryRestriction), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<SalesModelCategoryRestriction> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(SalesModelCategoryRestriction), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<SalesModelCategoryRestriction> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<SalesModelCategoryRestriction>(x => ids.Contains(x.Id));

            return Specs.Map.Facts.ToProjectAggregate.BySalesModelCategoryRestriction(specification)
                        .Map(_query)
                        .Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Project), x))
                        .ToArray();
        }
    }
}