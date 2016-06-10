using System.Collections.Generic;
using System.Linq;


using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class PriceAccessor : IStorageBasedDataObjectAccessor<Price>, IDataChangesHandler<Price>
    {
        private readonly IQuery _query;

        public PriceAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Price> GetSource() => Specs.Map.Erm.ToFacts.Price.Map(_query);

        public FindSpecification<Price> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Price>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Price> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Price), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Price> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Price), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Price> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Price), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Price> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<Price>(x => ids.Contains(x.Id));

            var ranges = _query.For<Price>()
                          .Where(specification)
                          .GroupBy(x => x.OrganizationUnitId, x => x.BeginDate)
                          .ToDictionary(x => x.Key, x => x.Min());

            var dates = _query.For<Order>()
                         .Select(x => new { Date = x.BeginDistributionDate, OrganizationUnitId = x.DestOrganizationUnitId })
                         .Union(_query.For<Order>().Select(x => new { Date = x.EndDistributionDateFact, OrganizationUnitId = x.DestOrganizationUnitId }))
                         .Union(_query.For<Price>().Select(x => new { Date = x.BeginDate, x.OrganizationUnitId }))
                         .GroupBy(x => x.OrganizationUnitId, x => x.Date)
                         .ToDictionary(x => x.Key, x => x.Distinct());

            var periodIds = ranges.Join(dates,
                                     x => x.Key,
                                     x => x.Key,
                                     (range, date) => date.Value
                                                          .Where(d => range.Value <= d)
                                                          .Select(d => new PeriodKey { OrganizationUnitId = range.Key, Start = d }))
                               .SelectMany(x => x)
                               .Distinct(PeriodKeyEqualityComparer.Instance);

            return periodIds.Select(x => new RelatedDataObjectOutdatedEvent<PeriodKey>(typeof(Price), x)).ToArray();
        }
    }
}