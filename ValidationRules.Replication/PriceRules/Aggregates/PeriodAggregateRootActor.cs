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
                        MessageTypeCode.AssociatedPositionsGroupCount,
                        MessageTypeCode.MaximumAdvertisementAmount,
                        MessageTypeCode.MinimalAdvertisementRestrictionShouldBeSpecified,
                        MessageTypeCode.MinimumAdvertisementAmount,
                    };

            public IQueryable<Period> GetSource()
            {
                // не менять код, он выверен до буквы
                var dates = _query.For<Facts::Order>().Select(x => new { Date = x.BeginDistribution, OrganizationUnitId = x.DestOrganizationUnitId })
                                  .Union(_query.For<Facts::Order>().Select(x => new { Date = x.EndDistributionFact, OrganizationUnitId = x.DestOrganizationUnitId }))
                                  .Union(_query.For<Facts::Order>().Select(x => new { Date = x.EndDistributionPlan, OrganizationUnitId = x.DestOrganizationUnitId }))
                                  .Union(_query.For<Facts::Price>().Select(x => new { Date = x.BeginDate, x.OrganizationUnitId }))
                                  .SelectMany(x => _query.For<Facts::Project>().Where(p => p.OrganizationUnitId == x.OrganizationUnitId),
                                              (x, p) => new { x.Date, x.OrganizationUnitId, ProjectId = p.Id })
                                  .OrderBy(x => x.Date);

                return dates.Select(x => new { Start = x, End = dates.FirstOrDefault(y => y.Date > x.Date && y.OrganizationUnitId == x.OrganizationUnitId) })
                                  .Select(x => new Period
                                  {
                                      Start = x.Start.Date,
                                      End = x.End != null ? x.End.Date : DateTime.MaxValue,
                                      OrganizationUnitId = x.Start.OrganizationUnitId,
                                      ProjectId = x.Start.ProjectId
                                  });
            }


            public FindSpecification<Period> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<SyncPeriodDataObjectCommand>().Select(c => c.PeriodKey).Distinct().ToArray();
                return Specs.Find.Aggs.Periods(aggregateIds);
            }
        }
    }
}