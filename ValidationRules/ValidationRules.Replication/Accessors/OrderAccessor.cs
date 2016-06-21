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
    public sealed class OrderAccessor : IStorageBasedDataObjectAccessor<Order>, IDataChangesHandler<Order>
    {
        private readonly IQuery _query;

        public OrderAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Order> GetSource() => Specs.Map.Erm.ToFacts.Order.Map(_query);

        public FindSpecification<Order> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Order>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Order> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Order), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Order> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Order), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Order> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Order), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Order> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<Order>(x => ids.Contains(x.Id));

            var ranges = _query.For<Order>()
                          .Where(specification)
                          .GroupBy(x => x.DestOrganizationUnitId, x => new { Start = x.BeginDistributionDate, End = x.EndDistributionDatePlan })
                          .ToDictionary(x => x.Key, x => x.Distinct());

            var dates = _query.For<Order>()
                         .Select(x => new { Date = x.BeginDistributionDate, OrganizationUnitId = x.DestOrganizationUnitId })
                         .Union(_query.For<Order>().Select(x => new { Date = x.EndDistributionDateFact, OrganizationUnitId = x.DestOrganizationUnitId }))
                         .Union(_query.For<Order>().Select(x => new { Date = x.EndDistributionDatePlan, OrganizationUnitId = x.DestOrganizationUnitId }))
                         .Union(_query.For<Price>().Select(x => new { Date = x.BeginDate, x.OrganizationUnitId }))
                         .GroupBy(x => x.OrganizationUnitId, x => x.Date)
                         .ToDictionary(x => x.Key, x => x.Distinct());

            var periodIds = ranges.Join(dates,
                                     x => x.Key,
                                     x => x.Key,
                                     (range, date) => date.Value
                                                          .Where(d => range.Value.Any(r => r.Start <= d && d <= r.End))
                                                          .Select(d => new PeriodKey { OrganizationUnitId = range.Key, Start = d }))
                               .SelectMany(x => x)
                               .Distinct(PeriodKeyEqualityComparer.Instance);

            return periodIds.Select(x => new RelatedDataObjectOutdatedEvent<PeriodKey>(typeof(Order), x)).ToArray();
        }
    }
}