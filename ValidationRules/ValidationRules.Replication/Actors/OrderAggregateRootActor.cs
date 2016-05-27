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
    public sealed class OrderAggregateRootActor : EntityActorBase<Order>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<OrderPosition> _orderPositionBulkRepository;
        private readonly IBulkRepository<OrderPricePosition> _orderPricePositionBulkRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;

        public OrderAggregateRootActor(
            IQuery query,
            IBulkRepository<Order> bulkRepository,
            IBulkRepository<OrderPosition> orderPositionBulkRepository,
            IBulkRepository<OrderPricePosition> orderPricePositionBulkRepository,
        IEqualityComparerFactory equalityComparerFactory)
            : base(query, bulkRepository, equalityComparerFactory, new OrderAccessor(query))
        {
            _query = query;
            _orderPositionBulkRepository = orderPositionBulkRepository;
            _orderPricePositionBulkRepository = orderPricePositionBulkRepository;
            _equalityComparerFactory = equalityComparerFactory;
        }


        public IReadOnlyCollection<IEntityActor> GetEntityActors() => new IEntityActor[0];

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<OrderPosition>(_query, _orderPositionBulkRepository, _equalityComparerFactory, new OrderPositionAccessor(_query)),
                    new ValueObjectActor<OrderPricePosition>(_query, _orderPricePositionBulkRepository, _equalityComparerFactory, new OrderPricePositionAccessor(_query)),
                };

        public sealed class OrderAccessor : IStorageBasedDataObjectAccessor<Order>
        {
            private readonly IQuery _query;

            public OrderAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order> GetSource() => Specs.Map.Facts.ToAggregates.Orders.Map(_query);

            public FindSpecification<Order> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order>(x => aggregateIds.Contains(x.Id));
            }
        }

        public sealed class OrderPositionAccessor : IStorageBasedDataObjectAccessor<OrderPosition>
        {
            private readonly IQuery _query;

            public OrderPositionAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<OrderPosition> GetSource() => Specs.Map.Facts.ToAggregates.OrderPositions.Map(_query);

            public FindSpecification<OrderPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return Specs.Find.Aggs.OrderPositions(aggregateIds);
            }
        }

        public sealed class OrderPricePositionAccessor : IStorageBasedDataObjectAccessor<OrderPricePosition>
        {
            private readonly IQuery _query;

            public OrderPricePositionAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<OrderPricePosition> GetSource() => Specs.Map.Facts.ToAggregates.OrderPricePositions.Map(_query);

            public FindSpecification<OrderPricePosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return Specs.Find.Aggs.OrderPricePositions(aggregateIds);
            }
        }
    }
}