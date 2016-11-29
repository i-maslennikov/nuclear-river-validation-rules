using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.PriceRules.Facts;

namespace NuClear.ValidationRules.Replication.PriceRules.Aggregates
{
    public sealed class OrderAggregateRootActor : AggregateRootActor<Order>
    {
        public OrderAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Order> bulkRepository,
            IBulkRepository<OrderPosition> orderPositionBulkRepository,
            IBulkRepository<OrderPricePosition> orderPricePositionBulkRepository,
            IBulkRepository<AmountControlledPosition> amountControlledPositionBulkRepository,
            IBulkRepository<OrderDeniedPosition> orderDeniedPositionBulkRepository,
            IBulkRepository<OrderAssociatedPosition> orderAssociatedPositionBulkRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new OrderAccessor(query), bulkRepository,
                HasValueObject(new OrderPositionAccessor(query), orderPositionBulkRepository),
                HasValueObject(new OrderPricePositionAccessor(query), orderPricePositionBulkRepository),
                HasValueObject(new AmountControlledPositionAccessor(query), amountControlledPositionBulkRepository),
                HasValueObject(new OrderDeniedPositionAccessor(query), orderDeniedPositionBulkRepository),
                HasValueObject(new OrderAssociatedPositionAccessor(query), orderAssociatedPositionBulkRepository));
        }

        public sealed class OrderAccessor : DataChangesHandler<Order>, IStorageBasedDataObjectAccessor<Order>
        {
            private readonly IQuery _query;

            public OrderAccessor(IQuery query) : base(CreateInvalidator())
            {

                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited,
                        MessageTypeCode.AdvertisementCountPerThemeShouldBeLimited,
                        MessageTypeCode.AssociatedPositionWithoutPrincipal,
                        MessageTypeCode.ConflictingPrincipalPosition,
                        MessageTypeCode.DeniedPositionsCheck,
                        MessageTypeCode.LinkedObjectsMissedInPrincipals,
                        MessageTypeCode.MaximumAdvertisementAmount,
                        MessageTypeCode.MinimumAdvertisementAmount,
                        MessageTypeCode.OrderPositionCorrespontToInactivePosition,
                        MessageTypeCode.OrderPositionShouldCorrespontToActualPrice,
                        MessageTypeCode.OrderPositionsShouldCorrespontToActualPrice,
                        MessageTypeCode.SatisfiedPrincipalPositionDifferentOrder,
                    };

            public IQueryable<Order> GetSource()
                => from order in _query.For<Facts::Order>()
                   select new Order { Id = order.Id, FirmId = order.FirmId, Number = order.Number, };

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

        public sealed class OrderPositionAccessor : DataChangesHandler<OrderPosition>, IStorageBasedDataObjectAccessor<OrderPosition>
        {
            private readonly IQuery _query;

            public OrderPositionAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited,
                        MessageTypeCode.AdvertisementCountPerThemeShouldBeLimited,
                        MessageTypeCode.AssociatedPositionWithoutPrincipal,
                        MessageTypeCode.ConflictingPrincipalPosition,
                        MessageTypeCode.DeniedPositionsCheck,
                        MessageTypeCode.LinkedObjectsMissedInPrincipals,
                        MessageTypeCode.SatisfiedPrincipalPositionDifferentOrder,
                    };

            public IQueryable<OrderPosition> GetSource()
            {
                var opas = from order in _query.For<Facts::Order>() // Чтобы сократить число позиций
                           join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                           join pricePosition in _query.For<Facts::PricePosition>() on orderPosition.PricePositionId equals pricePosition.Id
                           join opa in _query.For<Facts::OrderPositionAdvertisement>() on orderPosition.Id equals opa.OrderPositionId
                           join position in _query.For<Facts::Position>() on opa.PositionId equals position.Id
                           from category in _query.For<Facts::Category>().Where(x => x.Id == opa.CategoryId).DefaultIfEmpty()
                           select new OrderPosition
                           {
                               OrderId = orderPosition.OrderId,
                               OrderPositionId = orderPosition.Id,
                               ItemPositionId = position.Id,

                               PackagePositionId = pricePosition.PositionId,

                               HasNoBinding = opa.CategoryId == null && opa.FirmAddressId == null,
                               Category3Id = category.L3Id,
                               FirmAddressId = opa.FirmAddressId,
                               Category1Id = category.L1Id,

                               ThemeId = opa.ThemeId,
                           };

                var pkgs = from order in _query.For<Facts::Order>() // Чтобы сократить число позиций
                           join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                           join pricePosition in _query.For<Facts::PricePosition>() on orderPosition.PricePositionId equals pricePosition.Id
                           join opa in _query.For<Facts::OrderPositionAdvertisement>() on orderPosition.Id equals opa.OrderPositionId
                           join position in _query.For<Facts::Position>().Where(x => x.IsComposite) on pricePosition.PositionId equals position.Id
                           from category in _query.For<Facts::Category>().Where(x => x.Id == opa.CategoryId).DefaultIfEmpty()
                           select new OrderPosition
                           {
                               OrderId = orderPosition.OrderId,
                               OrderPositionId = orderPosition.Id,
                               ItemPositionId = pricePosition.PositionId,

                               PackagePositionId = pricePosition.PositionId,

                               HasNoBinding = opa.CategoryId == null && opa.FirmAddressId == null,
                               Category3Id = category.L3Id,
                               FirmAddressId = opa.FirmAddressId,
                               Category1Id = category.L1Id,

                               ThemeId = opa.ThemeId,
                           };

                return pkgs.Union(opas);
            }

            public FindSpecification<OrderPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<OrderPosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderPricePositionAccessor : DataChangesHandler<OrderPricePosition>, IStorageBasedDataObjectAccessor<OrderPricePosition>
        {
            private readonly IQuery _query;

            public OrderPricePositionAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderPositionCorrespontToInactivePosition,
                        MessageTypeCode.OrderPositionShouldCorrespontToActualPrice,
                    };

            public IQueryable<OrderPricePosition> GetSource()
                =>
                    from order in _query.For<Facts::Order>() // Чтобы сократить число позиций
                    join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
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
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<OrderPricePosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class AmountControlledPositionAccessor : DataChangesHandler<AmountControlledPosition>, IStorageBasedDataObjectAccessor<AmountControlledPosition>
        {
            private readonly IQuery _query;

            public AmountControlledPositionAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.MaximumAdvertisementAmount,
                        MessageTypeCode.MinimumAdvertisementAmount,
                    };

            public IQueryable<AmountControlledPosition> GetSource()
                => from order in _query.For<Facts::Order>() // Чтобы сократить число позиций
                   join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                   join adv in _query.For<Facts::OrderPositionAdvertisement>() on orderPosition.Id equals adv.OrderPositionId
                   join position in _query.For<Facts::Position>().Where(x => x.IsControlledByAmount) on adv.PositionId equals position.Id
                   select new AmountControlledPosition
                       {
                           OrderId = orderPosition.OrderId,
                           CategoryCode = position.CategoryCode,
                       };

            public FindSpecification<AmountControlledPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<AmountControlledPosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderDeniedPositionAccessor : DataChangesHandler<OrderDeniedPosition>, IStorageBasedDataObjectAccessor<OrderDeniedPosition>
        {
            private const int RulesetRuleTypeDenied = 2;

            private readonly IQuery _query;

            public OrderDeniedPositionAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.DeniedPositionsCheck
                    };

            public IQueryable<OrderDeniedPosition> GetSource()
            {
                var opas =
                    from order in _query.For<Facts::Order>() // Чтобы сократить число позиций
                    join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                    join pricePosition in _query.For<Facts::PricePosition>() on orderPosition.PricePositionId equals pricePosition.Id
                    join opa in _query.For<Facts::OrderPositionAdvertisement>() on orderPosition.Id equals opa.OrderPositionId
                    from category in _query.For<Facts::Category>().Where(x => x.Id == opa.CategoryId).DefaultIfEmpty()
                    select new
                        {
                            PriceId = pricePosition.PriceId,
                            OrderId = orderPosition.OrderId,
                            CauseOrderPositionId = orderPosition.Id,
                            CausePackagePositionId = pricePosition.PositionId,
                            CauseItemPositionId = opa.PositionId,
                            HasNoBinding = opa.CategoryId == null && opa.FirmAddressId == null,
                            Category3Id = category.L3Id,
                            FirmAddressId = opa.FirmAddressId,
                            Category1Id = category.L1Id,
                            Source = "opas",
                        };

                var pkgs =
                    from order in _query.For<Facts::Order>() // Чтобы сократить число позиций
                    join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                    join pricePosition in _query.For<Facts::PricePosition>() on orderPosition.PricePositionId equals pricePosition.Id
                    join opa in _query.For<Facts::OrderPositionAdvertisement>() on orderPosition.Id equals opa.OrderPositionId
                    join position in _query.For<Facts::Position>().Where(x => x.IsComposite) on pricePosition.PositionId equals position.Id
                    from category in _query.For<Facts::Category>().Where(x => x.Id == opa.CategoryId).DefaultIfEmpty()
                    select new
                        {
                            PriceId = pricePosition.PriceId,
                            OrderId = orderPosition.OrderId,
                            CauseOrderPositionId = orderPosition.Id,
                            CausePackagePositionId = pricePosition.PositionId,
                            CauseItemPositionId = pricePosition.PositionId,
                            HasNoBinding = opa.CategoryId == null && opa.FirmAddressId == null,
                            Category3Id = category.L3Id,
                            FirmAddressId = opa.FirmAddressId,
                            Category1Id = category.L1Id,
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
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<OrderDeniedPosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderAssociatedPositionAccessor : DataChangesHandler<OrderAssociatedPosition>, IStorageBasedDataObjectAccessor<OrderAssociatedPosition>
        {
            private const int RulesetRuleTypeAssociated = 1;

            private readonly IQuery _query;

            public OrderAssociatedPositionAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.AssociatedPositionWithoutPrincipal,
                        MessageTypeCode.ConflictingPrincipalPosition,
                        MessageTypeCode.LinkedObjectsMissedInPrincipals,
                        MessageTypeCode.SatisfiedPrincipalPositionDifferentOrder,
                    };

            public IQueryable<OrderAssociatedPosition> GetSource()
            {
                var opas =
                    from order in _query.For<Facts::Order>() // Чтобы сократить число позиций
                    join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                    join pricePosition in _query.For<Facts::PricePosition>() on orderPosition.PricePositionId equals pricePosition.Id
                    join opa in _query.For<Facts::OrderPositionAdvertisement>() on orderPosition.Id equals opa.OrderPositionId
                    from category in _query.For<Facts::Category>().Where(x => x.Id == opa.CategoryId).DefaultIfEmpty()
                    select new
                    {
                        PricePositionId = pricePosition.Id,
                        orderPosition.OrderId,
                        CauseOrderPositionId = orderPosition.Id,
                        CausePackagePositionId = pricePosition.PositionId,
                        CauseItemPositionId = opa.PositionId,
                        HasNoBinding = opa.CategoryId == null && opa.FirmAddressId == null,
                        Category3Id = category.L3Id,
                        opa.FirmAddressId,
                        Category1Id = category.L1Id,
                        Source = "opas",
                    };

                var pkgs =
                    from order in _query.For<Facts::Order>() // Чтобы сократить число позиций
                    join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                    join pricePosition in _query.For<Facts::PricePosition>() on orderPosition.PricePositionId equals pricePosition.Id
                    join opa in _query.For<Facts::OrderPositionAdvertisement>() on orderPosition.Id equals opa.OrderPositionId
                    join position in _query.For<Facts::Position>().Where(x => x.IsComposite) on pricePosition.PositionId equals position.Id
                    from category in _query.For<Facts::Category>().Where(x => x.Id == opa.CategoryId).DefaultIfEmpty()
                    select new
                    {
                        PricePositionId = pricePosition.Id,
                        orderPosition.OrderId,
                        CauseOrderPositionId = orderPosition.Id,
                        CausePackagePositionId = pricePosition.PositionId,
                        CauseItemPositionId = pricePosition.PositionId,
                        HasNoBinding = opa.CategoryId == null && opa.FirmAddressId == null,
                        Category3Id = category.L3Id,
                        opa.FirmAddressId,
                        Category1Id = category.L1Id,
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
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<OrderAssociatedPosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }
    }
}