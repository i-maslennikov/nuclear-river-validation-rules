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
    public sealed class OrderAggregateRootActor : EntityActorBase<Order>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<OrderPosition> _orderPositionBulkRepository;
        private readonly IBulkRepository<OrderPricePosition> _orderPricePositionBulkRepository;
        private readonly IBulkRepository<AmountControlledPosition> _amountControlledPositionBulkRepository;
        private readonly IBulkRepository<OrderDeniedPosition> _orderDeniedPositionBulkRepository;
        private readonly IBulkRepository<OrderAssociatedPosition> _orderAssociatedPositionBulkRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;

        public OrderAggregateRootActor(
            IQuery query,
            IBulkRepository<Order> bulkRepository,
            IBulkRepository<OrderPosition> orderPositionBulkRepository,
            IBulkRepository<OrderPricePosition> orderPricePositionBulkRepository,
            IBulkRepository<AmountControlledPosition> amountControlledPositionBulkRepository,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<OrderDeniedPosition> orderDeniedPositionBulkRepository,
            IBulkRepository<OrderAssociatedPosition> orderAssociatedPositionBulkRepository)
            : base(query, bulkRepository, equalityComparerFactory, new OrderAccessor(query))
        {
            _query = query;
            _orderPositionBulkRepository = orderPositionBulkRepository;
            _orderPricePositionBulkRepository = orderPricePositionBulkRepository;
            _equalityComparerFactory = equalityComparerFactory;
            _orderDeniedPositionBulkRepository = orderDeniedPositionBulkRepository;
            _orderAssociatedPositionBulkRepository = orderAssociatedPositionBulkRepository;
            _amountControlledPositionBulkRepository = amountControlledPositionBulkRepository;
        }


        public IReadOnlyCollection<IEntityActor> GetEntityActors() => new IEntityActor[0];

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<OrderPosition>(_query, _orderPositionBulkRepository, _equalityComparerFactory, new OrderPositionAccessor(_query)),
                    new ValueObjectActor<OrderPricePosition>(_query, _orderPricePositionBulkRepository, _equalityComparerFactory, new OrderPricePositionAccessor(_query)),
                    new ValueObjectActor<AmountControlledPosition>(_query, _amountControlledPositionBulkRepository, _equalityComparerFactory, new AmountControlledPositionAccessor(_query)),
                    new ValueObjectActor<OrderDeniedPosition>(_query, _orderDeniedPositionBulkRepository, _equalityComparerFactory, new OrderDeniedPositionAccessor(_query)),
                    new ValueObjectActor<OrderAssociatedPosition>(_query, _orderAssociatedPositionBulkRepository, _equalityComparerFactory, new OrderAssociatedPositionAccessor(_query)),
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

            public IQueryable<OrderPosition> GetSource()
            {
                var categories =
                    from c3 in _query.For<Facts::Category>()
                    join c2 in _query.For<Facts::Category>() on c3.ParentId equals c2.Id
                    join c1 in _query.For<Facts::Category>() on c2.ParentId equals c1.Id
                    select new { Category3Id = c3.Id, Category1Id = c1.Id };

                var opas = from order in _query.For<Facts::Order>() // Чтобы сократить число позиций
                           join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                           join pricePosition in _query.For<Facts::PricePosition>() on orderPosition.PricePositionId equals pricePosition.Id
                           join opa in _query.For<Facts::OrderPositionAdvertisement>() on orderPosition.Id equals opa.OrderPositionId
                           join position in _query.For<Facts::Position>() on opa.PositionId equals position.Id
                           from category in categories.Where(x => x.Category3Id == opa.CategoryId).DefaultIfEmpty()
                           select new OrderPosition
                           {
                               OrderId = orderPosition.OrderId,
                               OrderPositionId = orderPosition.Id,
                               ItemPositionId = position.Id,

                               PackagePositionId = pricePosition.PositionId,

                               HasNoBinding = opa.CategoryId == null && opa.FirmAddressId == null,
                               Category3Id = opa.CategoryId,
                               FirmAddressId = opa.FirmAddressId,
                               Category1Id = category.Category1Id,
                           };

                var pkgs = from order in _query.For<Facts::Order>() // Чтобы сократить число позиций
                           join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                           join pricePosition in _query.For<Facts::PricePosition>() on orderPosition.PricePositionId equals pricePosition.Id
                           join opa in _query.For<Facts::OrderPositionAdvertisement>() on orderPosition.Id equals opa.OrderPositionId
                           join position in _query.For<Facts::Position>().Where(x => x.IsComposite) on pricePosition.PositionId equals position.Id
                           from category in categories.Where(x => x.Category3Id == opa.CategoryId).DefaultIfEmpty()
                           select new OrderPosition
                           {
                               OrderId = orderPosition.OrderId,
                               OrderPositionId = orderPosition.Id,
                               ItemPositionId = pricePosition.PositionId,

                               PackagePositionId = pricePosition.PositionId,

                               HasNoBinding = opa.CategoryId == null && opa.FirmAddressId == null,
                               Category3Id = opa.CategoryId,
                               FirmAddressId = opa.FirmAddressId,
                               Category1Id = category.Category1Id,
                           };

                return pkgs.Union(opas);
            }

            public FindSpecification<OrderPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<OrderPosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderPricePositionAccessor : IStorageBasedDataObjectAccessor<OrderPricePosition>
        {
            private readonly IQuery _query;

            public OrderPricePositionAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<OrderPricePosition> GetSource()
                =>
                    from orderPosition in _query.For<Facts::OrderPosition>()
                    from pricePosition in _query.For<Facts::PricePosition>().Where(x => x.Id == orderPosition.PricePositionId).DefaultIfEmpty()
                    from pricePositionNotActive in _query.For<Facts::PricePositionNotActive>().Where(x => x.Id == orderPosition.PricePositionId).DefaultIfEmpty()
                    from position in _query.For<Facts::Position>().Where(x => x.Id == pricePosition.PositionId || x.Id == pricePositionNotActive.PositionId).DefaultIfEmpty()
                    select new OrderPricePosition
                    {
                        OrderId = orderPosition.OrderId,
                        OrderPositionId = orderPosition.Id,

                        OrderPositionName = position.Name,
                        PriceId = pricePosition != null ? pricePosition.PriceId : pricePositionNotActive != null ? pricePositionNotActive.PriceId : 0,
                        IsActive = pricePosition != null || pricePositionNotActive == null
                    };


            public FindSpecification<OrderPricePosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return Specs.Find.Aggs.OrderPricePositions(aggregateIds);
            }
        }

        public sealed class AmountControlledPositionAccessor : IStorageBasedDataObjectAccessor<AmountControlledPosition>
        {
            private readonly IQuery _query;

            public AmountControlledPositionAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<AmountControlledPosition> GetSource()
                => from orderPosition in _query.For<Facts::OrderPosition>()
                   join adv in _query.For<Facts::OrderPositionAdvertisement>() on orderPosition.Id equals adv.OrderPositionId
                   join position in _query.For<Facts::Position>().Where(x => x.IsControlledByAmount) on adv.PositionId equals position.Id
                   select new AmountControlledPosition
                       {
                           OrderId = orderPosition.OrderId,
                           CategoryCode = position.CategoryCode,
                       };

            public FindSpecification<AmountControlledPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<AmountControlledPosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderDeniedPositionAccessor : IStorageBasedDataObjectAccessor<OrderDeniedPosition>
        {
            private const int RulesetRuleTypeDenied = 2;

            private readonly IQuery _query;

            public OrderDeniedPositionAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<OrderDeniedPosition> GetSource()
            {
                var categories =
                    from c3 in _query.For<Facts::Category>()
                    join c2 in _query.For<Facts::Category>() on c3.ParentId equals c2.Id
                    join c1 in _query.For<Facts::Category>() on c2.ParentId equals c1.Id
                    select new { Category3Id = c3.Id, Category1Id = c1.Id };

                var opas =
                    from order in _query.For<Facts::Order>() // Чтобы сократить число позиций
                    join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                    join pricePosition in _query.For<Facts::PricePosition>() on orderPosition.PricePositionId equals pricePosition.Id
                    join opa in _query.For<Facts::OrderPositionAdvertisement>() on orderPosition.Id equals opa.OrderPositionId
                    from category in categories.Where(x => x.Category3Id == opa.CategoryId).DefaultIfEmpty()
                    select new
                        {
                            PriceId = pricePosition.PriceId,
                            OrderId = orderPosition.OrderId,
                            CauseOrderPositionId = orderPosition.Id,
                            CausePackagePositionId = pricePosition.PositionId,
                            CauseItemPositionId = opa.PositionId,
                            HasNoBinding = opa.CategoryId == null && opa.FirmAddressId == null,
                            Category3Id = opa.CategoryId,
                            FirmAddressId = opa.FirmAddressId,
                            Category1Id = category.Category1Id,
                            Source = "opas",
                        };

                var pkgs =
                    from order in _query.For<Facts::Order>() // Чтобы сократить число позиций
                    join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                    join pricePosition in _query.For<Facts::PricePosition>() on orderPosition.PricePositionId equals pricePosition.Id
                    join opa in _query.For<Facts::OrderPositionAdvertisement>() on orderPosition.Id equals opa.OrderPositionId
                    join position in _query.For<Facts::Position>().Where(x => x.IsComposite) on pricePosition.PositionId equals position.Id
                    from category in categories.Where(x => x.Category3Id == opa.CategoryId).DefaultIfEmpty()
                    select new
                        {
                            PriceId = pricePosition.PriceId,
                            OrderId = orderPosition.OrderId,
                            CauseOrderPositionId = orderPosition.Id,
                            CausePackagePositionId = pricePosition.PositionId,
                            CauseItemPositionId = pricePosition.PositionId,
                            HasNoBinding = opa.CategoryId == null && opa.FirmAddressId == null,
                            Category3Id = opa.CategoryId,
                            FirmAddressId = opa.FirmAddressId,
                            Category1Id = category.Category1Id,
                            Source = "pkgs",
                        };

                var deniedByPrice =
                    from bingingObject in opas.Union(pkgs)
                    join denied in _query.For<Facts::DeniedPosition>()
                        on new { bingingObject.PriceId, PositionId = bingingObject.CauseItemPositionId } equals new { denied.PriceId, denied.PositionId }
                    select new OrderDeniedPosition
                        {
                            OrderId = bingingObject.OrderId,
                            CauseOrderPositionId = bingingObject.CauseOrderPositionId,
                            CausePackagePositionId = bingingObject.CausePackagePositionId,
                            CauseItemPositionId = bingingObject.CauseItemPositionId,
                            HasNoBinding = bingingObject.HasNoBinding,
                            Category3Id = bingingObject.Category3Id,
                            FirmAddressId = bingingObject.FirmAddressId,
                            Category1Id = bingingObject.Category1Id,

                            DeniedPositionId = denied.PositionDeniedId,
                            BindingType = denied.ObjectBindingType,

                            Source = bingingObject.Source + " by price",
                        };

                var deniedByRuleset =
                    from bingingObject in opas.Union(pkgs)
                    join denied in _query.For<Facts::RulesetRule>().Where(x => x.RuleType == RulesetRuleTypeDenied)
                        on bingingObject.CauseItemPositionId equals denied.DependentPositionId
                    select new OrderDeniedPosition
                    {
                        OrderId = bingingObject.OrderId,
                        CauseOrderPositionId = bingingObject.CauseOrderPositionId,
                        CausePackagePositionId = bingingObject.CausePackagePositionId,
                        CauseItemPositionId = bingingObject.CauseItemPositionId,
                        HasNoBinding = bingingObject.HasNoBinding,
                        Category3Id = bingingObject.Category3Id,
                        FirmAddressId = bingingObject.FirmAddressId,
                        Category1Id = bingingObject.Category1Id,

                        DeniedPositionId = denied.PrincipalPositionId,
                        BindingType = denied.ObjectBindingType,

                        Source = bingingObject.Source + " by ruleset",
                    };

                return deniedByPrice.Union(deniedByRuleset);
            }

            public FindSpecification<OrderDeniedPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<OrderDeniedPosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderAssociatedPositionAccessor : IStorageBasedDataObjectAccessor<OrderAssociatedPosition>
        {
            private const int RulesetRuleTypeAssociated = 1;

            private readonly IQuery _query;

            public OrderAssociatedPositionAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<OrderAssociatedPosition> GetSource()
            {
                var categories =
                    from c3 in _query.For<Facts::Category>()
                    join c2 in _query.For<Facts::Category>() on c3.ParentId equals c2.Id
                    join c1 in _query.For<Facts::Category>() on c2.ParentId equals c1.Id
                    select new { Category3Id = c3.Id, Category1Id = c1.Id };

                var opas =
                    from order in _query.For<Facts::Order>() // Чтобы сократить число позиций
                    join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                    join pricePosition in _query.For<Facts::PricePosition>() on orderPosition.PricePositionId equals pricePosition.Id
                    join opa in _query.For<Facts::OrderPositionAdvertisement>() on orderPosition.Id equals opa.OrderPositionId
                    from category in categories.Where(x => x.Category3Id == opa.CategoryId).DefaultIfEmpty()
                    select new
                    {
                        PricePositionId = pricePosition.Id,
                        orderPosition.OrderId,
                        CauseOrderPositionId = orderPosition.Id,
                        CausePackagePositionId = pricePosition.PositionId,
                        CauseItemPositionId = opa.PositionId,
                        HasNoBinding = opa.CategoryId == null && opa.FirmAddressId == null,
                        Category3Id = opa.CategoryId,
                        opa.FirmAddressId,
                        category.Category1Id,
                        Source = "opas",
                    };

                var pkgs =
                    from order in _query.For<Facts::Order>() // Чтобы сократить число позиций
                    join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                    join pricePosition in _query.For<Facts::PricePosition>() on orderPosition.PricePositionId equals pricePosition.Id
                    join opa in _query.For<Facts::OrderPositionAdvertisement>() on orderPosition.Id equals opa.OrderPositionId
                    join position in _query.For<Facts::Position>().Where(x => x.IsComposite) on pricePosition.PositionId equals position.Id
                    from category in categories.Where(x => x.Category3Id == opa.CategoryId).DefaultIfEmpty()
                    select new
                    {
                        PricePositionId = pricePosition.Id,
                        orderPosition.OrderId,
                        CauseOrderPositionId = orderPosition.Id,
                        CausePackagePositionId = pricePosition.PositionId,
                        CauseItemPositionId = pricePosition.PositionId,
                        HasNoBinding = opa.CategoryId == null && opa.FirmAddressId == null,
                        Category3Id = opa.CategoryId,
                        opa.FirmAddressId,
                        category.Category1Id,
                        Source = "pkgs",
                    };

                var associatedByPrice =
                    from bingingObject in opas.Union(pkgs)
                    join apg in _query.For<Facts::AssociatedPositionsGroup>() on bingingObject.PricePositionId equals apg.PricePositionId
                    join ap in _query.For<Facts::AssociatedPosition>() on apg.Id equals ap.AssociatedPositionsGroupId
                    select new OrderAssociatedPosition
                    {
                        OrderId = bingingObject.OrderId,
                        CauseOrderPositionId = bingingObject.CauseOrderPositionId,
                        CausePackagePositionId = bingingObject.CausePackagePositionId,
                        CauseItemPositionId = bingingObject.CauseItemPositionId,
                        HasNoBinding = bingingObject.HasNoBinding,
                        Category3Id = bingingObject.Category3Id,
                        FirmAddressId = bingingObject.FirmAddressId,
                        Category1Id = bingingObject.Category1Id,

                        PrincipalPositionId = ap.PositionId,
                        BindingType = ap.ObjectBindingType,

                        Source = bingingObject.Source + " by price",
                    };

                var associatedByRuleset =
                    from bingingObject in opas.Union(pkgs)
                    join associated in _query.For<Facts::RulesetRule>().Where(x => x.RuleType == RulesetRuleTypeAssociated)
                        on bingingObject.CauseItemPositionId equals associated.DependentPositionId
                    select new OrderAssociatedPosition
                    {
                        OrderId = bingingObject.OrderId,
                        CauseOrderPositionId = bingingObject.CauseOrderPositionId,
                        CausePackagePositionId = bingingObject.CausePackagePositionId,
                        CauseItemPositionId = bingingObject.CauseItemPositionId,
                        HasNoBinding = bingingObject.HasNoBinding,
                        Category3Id = bingingObject.Category3Id,
                        FirmAddressId = bingingObject.FirmAddressId,
                        Category1Id = bingingObject.Category1Id,

                        PrincipalPositionId = associated.PrincipalPositionId,
                        BindingType = associated.ObjectBindingType,

                        Source = bingingObject.Source + " by ruleset",
                    };

                return associatedByPrice.Union(associatedByRuleset);
            }

            public FindSpecification<OrderAssociatedPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<OrderAssociatedPosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }
    }
}