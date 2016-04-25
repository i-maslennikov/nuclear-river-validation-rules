using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Events;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Accessors
{
    public sealed class CategoryGroupAccessor : IStorageBasedDataObjectAccessor<CategoryGroup>, IDataChangesHandler<CategoryGroup>
    {
        private readonly IQuery _query;

        public CategoryGroupAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<CategoryGroup> GetSource() => Specs.Map.Erm.ToFacts.CategoryGroups.Map(_query);

        public FindSpecification<CategoryGroup> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            => new FindSpecification<CategoryGroup>(x => commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).Contains(x.Id));

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<CategoryGroup> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(CategoryGroup), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<CategoryGroup> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(CategoryGroup), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<CategoryGroup> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(CategoryGroup), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<CategoryGroup> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<CategoryGroup>(x => ids.Contains(x.Id));

            IEnumerable<IEvent> events = Specs.Map.Facts.ToFirmAggregate.ByCategoryGroup(specification)
                                              .Map(_query)
                                              .Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Firm), x));

            events = events.Concat(Specs.Map.Facts.ToClientAggregate.ByCategoryGroup(specification)
                                        .Map(_query)
                                        .Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Client), x)));
            return events.ToArray();
        }
    }
}