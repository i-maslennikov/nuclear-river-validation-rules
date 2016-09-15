using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.PriceRules.Facts;

namespace NuClear.ValidationRules.Replication.PriceRules.Aggregates
{
    public sealed class PeriodAggregateRootActor : EntityActorBase<Period>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<PricePeriod> _pricePeriodBulkRepository;
        private readonly IBulkRepository<OrderPeriod> _orderPeriodBulkRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;

        public PeriodAggregateRootActor(
            IQuery query,
            IBulkRepository<Period> bulkRepository,
            IBulkRepository<PricePeriod> pricePeriodBulkRepository,
            IBulkRepository<OrderPeriod> orderPeriodBulkRepository,
            IEqualityComparerFactory equalityComparerFactory)
            : base(query, bulkRepository, equalityComparerFactory, new PeriodAccessor(query))
        {
            _query = query;
            _pricePeriodBulkRepository = pricePeriodBulkRepository;
            _orderPeriodBulkRepository = orderPeriodBulkRepository;
            _equalityComparerFactory = equalityComparerFactory;
        }


        public IReadOnlyCollection<IEntityActor> GetEntityActors() => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<PricePeriod>(_query, _pricePeriodBulkRepository, _equalityComparerFactory, new PricePeriodAccessor(_query)),
                    new ValueObjectActor<OrderPeriod>(_query, _orderPeriodBulkRepository, _equalityComparerFactory, new OrderPeriodAccessor(_query)),
                };

        public sealed class PeriodAccessor : IStorageBasedDataObjectAccessor<Period>
        {
            private readonly IQuery _query;

            public PeriodAccessor(IQuery query)
            {
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
                var aggregateIds = commands.OfType<ReplacePeriodValueObjectCommand>().Select(c => c.PeriodKey).Distinct().ToArray();
                return Specs.Find.Aggs.Periods(aggregateIds);
            }
        }

        public sealed class PricePeriodAccessor : IStorageBasedDataObjectAccessor<PricePeriod>
        {
            private readonly IQuery _query;

            public PricePeriodAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<PricePeriod> GetSource() => Specs.Map.Facts.ToAggregates.PricePeriods.Map(_query);

            public FindSpecification<PricePeriod> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplacePeriodValueObjectCommand>().Select(c => c.PeriodKey).Distinct().ToArray();
                return Specs.Find.Aggs.PricePeriods(aggregateIds);
            }
        }

        public sealed class OrderPeriodAccessor : IStorageBasedDataObjectAccessor<OrderPeriod>
        {
            private const int OrderOnRegistration = 1;
            private const int OrderOnTermination = 4;
            private const int GlobalScope = 0;

            private readonly IQuery _query;

            public OrderPeriodAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<OrderPeriod> GetSource()
            {
                var dates = _query.For<Facts::Order>()
                                  .Select(x => new { Date = x.BeginDistributionDate, OrganizationUnitId = x.DestOrganizationUnitId })
                                  .Union(_query.For<Facts::Order>().Select(x => new { Date = x.EndDistributionDateFact, OrganizationUnitId = x.DestOrganizationUnitId }))
                                  .Union(_query.For<Facts::Order>().Select(x => new { Date = x.EndDistributionDatePlan, OrganizationUnitId = x.DestOrganizationUnitId }))
                                  .Union(_query.For<Facts::Price>().Select(x => new { Date = x.BeginDate, x.OrganizationUnitId }))
                                  .Distinct();

                // https://github.com/linq2db/linq2db/issues/356
                dates = dates.Select(x => new { x.Date, x.OrganizationUnitId });

                var result = _query.For<Facts::Order>()
                                   .SelectMany(order => dates.Where(date => date.OrganizationUnitId == order.DestOrganizationUnitId &&
                                                                            order.BeginDistributionDate <= date.Date && date.Date < order.EndDistributionDatePlan)
                                                             .Select(x => new OrderPeriod
                                                                 {
                                                                     OrderId = order.Id,
                                                                     OrganizationUnitId = order.DestOrganizationUnitId,
                                                                     Start = x.Date,
                                                                     Scope = order.WorkflowStepId == OrderOnTermination && order.EndDistributionDateFact <= x.Date
                                                                                 ? order.Id
                                                                                 : (order.WorkflowStepId == OrderOnRegistration ? order.Id : GlobalScope)
                                                                 }));

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