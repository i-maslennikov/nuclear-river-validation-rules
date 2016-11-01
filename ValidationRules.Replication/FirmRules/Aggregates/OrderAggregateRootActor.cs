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
using NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.FirmRules.Facts;

namespace NuClear.ValidationRules.Replication.FirmRules.Aggregates
{
    public sealed class OrderAggregateRootActor : EntityActorBase<Order>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<Order.FirmOrganiationUnitMismatch> _invalidFirmRepository;
        private readonly IBulkRepository<Order.NotApplicapleForDesktopPosition> _notApplicapleForDesktopPositionRepository;
        private readonly IBulkRepository<Order.SelfAdvertisementPosition> _selfAdvertisementPositionRepository;
        private readonly IBulkRepository<Order.CategoryPurchase> _categoryPurchaseRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;

        public OrderAggregateRootActor(
            IQuery query,
            IBulkRepository<Order> bulkRepository,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Order.FirmOrganiationUnitMismatch> invalidFirmRepository,
            IBulkRepository<Order.CategoryPurchase> categoryPurchaseRepository,
            IBulkRepository<Order.NotApplicapleForDesktopPosition> notApplicapleForDesktopPositionRepository,
            IBulkRepository<Order.SelfAdvertisementPosition> selfAdvertisementPositionRepository)
            : base(query, bulkRepository, equalityComparerFactory, new OrderAccessor(query))
        {
            _query = query;
            _equalityComparerFactory = equalityComparerFactory;
            _invalidFirmRepository = invalidFirmRepository;
            _categoryPurchaseRepository = categoryPurchaseRepository;
            _notApplicapleForDesktopPositionRepository = notApplicapleForDesktopPositionRepository;
            _selfAdvertisementPositionRepository = selfAdvertisementPositionRepository;
        }


        public IReadOnlyCollection<IEntityActor> GetEntityActors() => new IEntityActor[0];

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<Order.FirmOrganiationUnitMismatch>(_query, _invalidFirmRepository, _equalityComparerFactory, new OrderFirmOrganiationUnitMismatchAccessor(_query)),
                    new ValueObjectActor<Order.NotApplicapleForDesktopPosition>(_query, _notApplicapleForDesktopPositionRepository, _equalityComparerFactory, new NotApplicapleForDesktopPositionAccessor(_query)),
                    new ValueObjectActor<Order.SelfAdvertisementPosition>(_query, _selfAdvertisementPositionRepository, _equalityComparerFactory, new SelfAdvertisementPositionAccessor(_query)),
                    new ValueObjectActor<Order.CategoryPurchase>(_query, _categoryPurchaseRepository, _equalityComparerFactory, new OrderCategoryPurchaseAccessor(_query)),
                };

        public sealed class OrderAccessor : IStorageBasedDataObjectAccessor<Order>
        {
            private readonly IQuery _query;

            public OrderAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order> GetSource()
                => from order in _query.For<Facts::Order>()
                   from project in _query.For<Facts::Project>().Where(x => x.OrganizationUnitId == order.DestOrganizationUnitId)
                   select new Order
                       {
                           Id = order.Id,
                           Number = order.Number,
                           FirmId = order.FirmId,
                           ProjectId = project.Id,
                           Begin = order.BeginDistribution,
                           End = order.EndDistributionFact,
                           Scope = Scope.Compute(order.WorkflowStep, order.Id),
                       };

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

        public sealed class NotApplicapleForDesktopPositionAccessor : IStorageBasedDataObjectAccessor<Order.NotApplicapleForDesktopPosition>
        {
            private readonly IQuery _query;

            public NotApplicapleForDesktopPositionAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.NotApplicapleForDesktopPosition> GetSource()
                => (from orderPosition in _query.For<Facts::OrderPosition>()
                    from order in _query.For<Facts::Order>().Where(x => x.Id == orderPosition.OrderId)
                    from orderPositionAdvertisement in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id)
                    from position in _query.For<Facts::SpecialPosition>().Where(x => !x.IsApplicapleForPc).Where(x => x.Id == orderPositionAdvertisement.PositionId)
                    select new Order.NotApplicapleForDesktopPosition { OrderId = orderPosition.OrderId }).Distinct();

            public FindSpecification<Order.NotApplicapleForDesktopPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.NotApplicapleForDesktopPosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class SelfAdvertisementPositionAccessor : IStorageBasedDataObjectAccessor<Order.SelfAdvertisementPosition>
        {
            private readonly IQuery _query;

            public SelfAdvertisementPositionAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.SelfAdvertisementPosition> GetSource()
                => (from orderPosition in _query.For<Facts::OrderPosition>()
                    from order in _query.For<Facts::Order>().Where(x => x.Id == orderPosition.OrderId)
                    from orderPositionAdvertisement in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id)
                    from position in _query.For<Facts::SpecialPosition>().Where(x => x.IsSelfAdvertisementOnPc).Where(x => x.Id == orderPositionAdvertisement.PositionId)
                    select new Order.SelfAdvertisementPosition { OrderId = orderPosition.OrderId }).Distinct();

            public FindSpecification<Order.SelfAdvertisementPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.SelfAdvertisementPosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderFirmOrganiationUnitMismatchAccessor : IStorageBasedDataObjectAccessor<Order.FirmOrganiationUnitMismatch>
        {
            private readonly IQuery _query;

            public OrderFirmOrganiationUnitMismatchAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.FirmOrganiationUnitMismatch> GetSource()
                => from order in _query.For<Facts::Order>()
                   from firm in _query.For<Facts::Firm>().Where(x => x.Id == order.FirmId)
                   where order.DestOrganizationUnitId != firm.OrganizationUnitId
                   select new Order.FirmOrganiationUnitMismatch { OrderId = order.Id };

            public FindSpecification<Order.FirmOrganiationUnitMismatch> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.FirmOrganiationUnitMismatch>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderCategoryPurchaseAccessor : IStorageBasedDataObjectAccessor<Order.CategoryPurchase>
        {
            private readonly IQuery _query;

            public OrderCategoryPurchaseAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.CategoryPurchase> GetSource()
                => from order in _query.For<Facts::Order>()
                   from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                   from opa in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id).Where(x => x.CategoryId.HasValue)
                   select new Order.CategoryPurchase { OrderId = order.Id, CategoryId = opa.CategoryId.Value };


            public FindSpecification<Order.CategoryPurchase> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.CategoryPurchase>(x => aggregateIds.Contains(x.OrderId));
            }
        }
    }
}
