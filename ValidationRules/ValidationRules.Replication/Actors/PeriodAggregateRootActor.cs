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
using NuClear.ValidationRules.Storage.Model.Aggregates;

namespace NuClear.ValidationRules.Replication.Actors
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


        public IReadOnlyCollection<IEntityActor> GetEntityActors() => new IEntityActor[0];

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

            public IQueryable<Period> GetSource() => Specs.Map.Facts.ToAggregates.Periods.Map(_query);

            public FindSpecification<Period> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var periodKeys = commands.OfType<SyncPeriodDataObjectCommand>().Select(c => c.PeriodKey)
                                           .Distinct()
                                           .ToArray();

                var findSpecification = periodKeys.Aggregate(new FindSpecification<Period>(x => true),
                                                             (spec, periodKey) =>
                                                             spec |
                                                             new FindSpecification<Period>(x => x.OrganizationUnitId == periodKey.OrganizationUnitId && x.Start == periodKey.Start));
                return findSpecification;
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
                var aggregateIds = commands.Cast<ReplacePeriodValueObjectCommand>().Select(c => c.PeriodKey).Distinct().ToArray();
                return Specs.Find.Aggs.PricePeriods(aggregateIds);
            }
        }

        public sealed class OrderPeriodAccessor : IStorageBasedDataObjectAccessor<OrderPeriod>
        {
            private readonly IQuery _query;

            public OrderPeriodAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<OrderPeriod> GetSource() => Specs.Map.Facts.ToAggregates.OrderPeriods.Map(_query);

            public FindSpecification<OrderPeriod> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplacePeriodValueObjectCommand>().Select(c => c.PeriodKey).Distinct().ToArray();
                return Specs.Find.Aggs.OrderPeriods(aggregateIds);
            }
        }
    }
}