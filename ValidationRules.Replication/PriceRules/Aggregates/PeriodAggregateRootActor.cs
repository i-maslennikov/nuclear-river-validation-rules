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

using Facts = NuClear.ValidationRules.Storage.Model.PriceRules.Facts;

namespace NuClear.ValidationRules.Replication.PriceRules.Aggregates
{
    public sealed class PeriodAggregateRootActor : AggregateRootActor<Period>
    {
        public PeriodAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Period> bulkRepository,
            IBulkRepository<PricePeriod> pricePeriodBulkRepository,
            IBulkRepository<OrderPeriod> orderPeriodBulkRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new PeriodAccessor(query), bulkRepository,
                HasValueObject(new PricePeriodAccessor(query), pricePeriodBulkRepository),
                HasValueObject(new OrderPeriodAccessor(query), orderPeriodBulkRepository));
        }

        public sealed class PeriodAccessor : DataChangesHandler<Period>, IStorageBasedDataObjectAccessor<Period>
        {
            private readonly IQuery _query;

            public PeriodAccessor(IQuery query)
            {
                Invalidate(MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited);
                Invalidate(MessageTypeCode.AdvertisementCountPerThemeShouldBeLimited);
                Invalidate(MessageTypeCode.AssociatedPositionsGroupCount);
                Invalidate(MessageTypeCode.AssociatedPositionWithoutPrincipal);
                Invalidate(MessageTypeCode.ConflictingPrincipalPosition);
                Invalidate(MessageTypeCode.DeniedPositionsCheck);
                Invalidate(MessageTypeCode.LinkedObjectsMissedInPrincipals);
                Invalidate(MessageTypeCode.MaximumAdvertisementAmount);
                Invalidate(MessageTypeCode.MinimalAdvertisementRestrictionShouldBeSpecified);
                Invalidate(MessageTypeCode.MinimumAdvertisementAmount);
                Invalidate(MessageTypeCode.OrderPositionCorrespontToInactivePosition);
                Invalidate(MessageTypeCode.OrderPositionShouldCorrespontToActualPrice);
                Invalidate(MessageTypeCode.OrderPositionsShouldCorrespontToActualPrice);
                Invalidate(MessageTypeCode.SatisfiedPrincipalPositionDifferentOrder);

                _query = query;
            }

            public IQueryable<Period> GetSource()
            {
                var dates = _query.For<Facts::Order>().Select(x => new { Date = x.BeginDistributionDate, OrganizationUnitId = x.DestOrganizationUnitId })
                                  .Union(_query.For<Facts::Order>().Select(x => new { Date = x.EndDistributionDateFact, OrganizationUnitId = x.DestOrganizationUnitId }))
                                  .Union(_query.For<Facts::Order>().Select(x => new { Date = x.EndDistributionDatePlan, OrganizationUnitId = x.DestOrganizationUnitId }))
                                  .Union(_query.For<Facts::Price>().Select(x => new { Date = x.BeginDate, x.OrganizationUnitId }))
                                  .SelectMany(x => _query.For<Facts::Project>().Where(p => p.OrganizationUnitId == x.OrganizationUnitId).DefaultIfEmpty(),
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
                var aggregateIds = commands.OfType<SyncPeriodDataObjectCommand>().Select(c => c.PeriodKey).Distinct().ToArray();
                return Specs.Find.Aggs.Periods(aggregateIds);
            }
        }

        public sealed class PricePeriodAccessor : DataChangesHandler<PricePeriod>, IStorageBasedDataObjectAccessor<PricePeriod>
        {
            private readonly IQuery _query;

            public PricePeriodAccessor(IQuery query)
            {
                Invalidate(MessageTypeCode.AssociatedPositionsGroupCount);
                Invalidate(MessageTypeCode.MaximumAdvertisementAmount);
                Invalidate(MessageTypeCode.MinimalAdvertisementRestrictionShouldBeSpecified);
                Invalidate(MessageTypeCode.MinimumAdvertisementAmount);
                Invalidate(MessageTypeCode.OrderPositionShouldCorrespontToActualPrice);
                Invalidate(MessageTypeCode.OrderPositionsShouldCorrespontToActualPrice);

                _query = query;
            }

            public IQueryable<PricePeriod> GetSource()
            {
                var dates = _query.For<Facts::Order>()
                                  .Select(x => new { Date = x.BeginDistributionDate, OrganizationUnitId = x.DestOrganizationUnitId })
                                  .Union(_query.For<Facts::Order>().Select(x => new { Date = x.EndDistributionDateFact, OrganizationUnitId = x.DestOrganizationUnitId }))
                                  .Union(_query.For<Facts::Order>().Select(x => new { Date = x.EndDistributionDatePlan, OrganizationUnitId = x.DestOrganizationUnitId }))
                                  .Union(_query.For<Facts::Price>().Select(x => new { Date = x.BeginDate, x.OrganizationUnitId }));

                var prices =
                    from price in _query.For<Facts::Price>()
                    let nextPrice = _query.For<Facts::Price>().Where(x => x.OrganizationUnitId == price.OrganizationUnitId && x.BeginDate > price.BeginDate).Min(x => (DateTime?)x.BeginDate)
                    select new { price.Id, price.OrganizationUnitId, Begin = price.BeginDate, End = nextPrice ?? DateTime.MaxValue };

                var result =
                    from date in dates
                    from price in prices.Where(x => x.OrganizationUnitId == date.OrganizationUnitId && x.Begin <= date.Date && date.Date < x.End )
                    select new PricePeriod
                        {
                            OrganizationUnitId = date.OrganizationUnitId,
                            PriceId = price.Id,
                            Start = date.Date
                        };

                return result;
            }

            public FindSpecification<PricePeriod> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplacePeriodValueObjectCommand>().Select(c => c.PeriodKey).Distinct().ToArray();
                return Specs.Find.Aggs.PricePeriods(aggregateIds);
            }
        }

        public sealed class OrderPeriodAccessor : DataChangesHandler<OrderPeriod>, IStorageBasedDataObjectAccessor<OrderPeriod>
        {
            private readonly IQuery _query;

            public OrderPeriodAccessor(IQuery query)
            {
                Invalidate(MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited);
                Invalidate(MessageTypeCode.AdvertisementCountPerThemeShouldBeLimited);
                Invalidate(MessageTypeCode.AssociatedPositionWithoutPrincipal);
                Invalidate(MessageTypeCode.ConflictingPrincipalPosition);
                Invalidate(MessageTypeCode.DeniedPositionsCheck);
                Invalidate(MessageTypeCode.LinkedObjectsMissedInPrincipals);
                Invalidate(MessageTypeCode.MaximumAdvertisementAmount);
                Invalidate(MessageTypeCode.MinimalAdvertisementRestrictionShouldBeSpecified);
                Invalidate(MessageTypeCode.MinimumAdvertisementAmount);
                Invalidate(MessageTypeCode.OrderPositionCorrespontToInactivePosition);
                Invalidate(MessageTypeCode.OrderPositionShouldCorrespontToActualPrice);
                Invalidate(MessageTypeCode.OrderPositionsShouldCorrespontToActualPrice);
                Invalidate(MessageTypeCode.SatisfiedPrincipalPositionDifferentOrder);

                _query = query;
            }

            public IQueryable<OrderPeriod> GetSource()
            {
                var dates = _query.For<Facts::Order>()
                                  .Select(x => new { Date = x.BeginDistributionDate, OrganizationUnitId = x.DestOrganizationUnitId })
                                  .Union(_query.For<Facts::Order>().Select(x => new { Date = x.EndDistributionDateFact, OrganizationUnitId = x.DestOrganizationUnitId }))
                                  .Union(_query.For<Facts::Order>().Select(x => new { Date = x.EndDistributionDatePlan, OrganizationUnitId = x.DestOrganizationUnitId }))
                                  .Union(_query.For<Facts::Price>().Select(x => new { Date = x.BeginDate, x.OrganizationUnitId }));

                var result =
                    from order in _query.For<Facts::Order>()
                    from date in dates.Where(date => date.OrganizationUnitId == order.DestOrganizationUnitId && order.BeginDistributionDate <= date.Date && date.Date < order.EndDistributionDatePlan)
                    select new OrderPeriod
                        {
                            OrderId = order.Id,
                            OrganizationUnitId = order.DestOrganizationUnitId,
                            Start = date.Date,
                            Scope = order.EndDistributionDateFact > date.Date ? Scope.Compute(order.WorkflowStepId, order.Id) : order.Id
                        };

                return result;
            }

            public FindSpecification<OrderPeriod> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplacePeriodValueObjectCommand>().Select(c => c.PeriodKey).Distinct().ToArray();
                return Specs.Find.Aggs.OrderPeriods(aggregateIds);
            }
        }
    }
}