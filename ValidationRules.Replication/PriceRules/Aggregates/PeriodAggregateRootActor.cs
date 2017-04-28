using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.PriceRules.Aggregates
{
    public sealed class PeriodAggregateRootActor : AggregateRootActor<Period>
    {
        public PeriodAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Period> bulkRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new PeriodAccessor(query), bulkRepository);
        }

        public sealed class PeriodAccessor : DataChangesHandler<Period>, IStorageBasedDataObjectAccessor<Period>
        {
            private readonly IQuery _query;

            public PeriodAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited,
                        MessageTypeCode.AdvertisementCountPerThemeShouldBeLimited,
                        MessageTypeCode.MaximumAdvertisementAmount,
                        MessageTypeCode.MinimumAdvertisementAmount,
                    };

            public IQueryable<Period> GetSource()
            {
                var dates =
                    _query.For<Facts::Order>().Select(x => new { Date = x.BeginDistribution })
                          .Union(_query.For<Facts::Order>().Select(x => new { Date = x.EndDistributionFact }))
                          .Union(_query.For<Facts::Order>().Select(x => new { Date = x.EndDistributionPlan }))
                          .Union(_query.For<Facts::Price>().Select(x => new { Date = x.BeginDate }));

                var result =
                    from date in dates
                    from next in dates.Where(x => x.Date > date.Date).OrderBy(x => x.Date).Take(1).DefaultIfEmpty()
                    select new Period { Start = date.Date, End = next != null ? next.Date : DateTime.MaxValue };

                return result;
            }

            public FindSpecification<Period> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var dates = commands.Cast<SyncPeriodCommand>().Select(c => c.Date).Distinct();
                return Periods(dates);
            }

            public static FindSpecification<Period> Periods(IEnumerable<DateTime> aggregateIds)
            {
                var result = new FindSpecification<Period>(x => false);

                return aggregateIds.Select(PeriodSpecificationForSingleKey)
                                   .Aggregate(result, (current, spec) => current | spec);
            }

            private static FindSpecification<Period> PeriodSpecificationForSingleKey(DateTime date)
                => new FindSpecification<Period>(x => x.Start <= date && date <= x.End);

        }
    }
}