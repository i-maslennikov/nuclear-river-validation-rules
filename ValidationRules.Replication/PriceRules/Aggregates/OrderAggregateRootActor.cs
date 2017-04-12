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

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.PriceRules.Aggregates
{
    public sealed class OrderAggregateRootActor : AggregateRootActor<Order>
    {
        public OrderAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Order> bulkRepository,
            IBulkRepository<Order.OrderPricePosition> orderPricePositionBulkRepository,
            IBulkRepository<Order.OrderCategoryPosition> orderCategoryPositionBulkRepository,
            IBulkRepository<Order.OrderThemePosition> orderThemePositionBulkRepository,
            IBulkRepository<Order.AmountControlledPosition> amountControlledPositionBulkRepository,
            IBulkRepository<Order.ActualPrice> actualPriceBulkRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new OrderAccessor(query), bulkRepository,
                HasValueObject(new OrderPricePositionAccessor(query), orderPricePositionBulkRepository),
                HasValueObject(new OrderCategoryPositionAccessor(query), orderCategoryPositionBulkRepository),
                HasValueObject(new OrderThemePositionAccessor(query), orderThemePositionBulkRepository),
                HasValueObject(new AmountControlledPositionAccessor(query), amountControlledPositionBulkRepository),
                HasValueObject(new ActualPriceAccessor(query), actualPriceBulkRepository));
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
                        MessageTypeCode.MaximumAdvertisementAmount,
                        MessageTypeCode.MinimumAdvertisementAmount,
                        MessageTypeCode.OrderPositionCorrespontToInactivePosition,
                        MessageTypeCode.OrderPositionMayCorrespontToActualPrice,
                        MessageTypeCode.OrderPositionMustCorrespontToActualPrice,
                        MessageTypeCode.OrderMustHaveActualPrice,
                    };

            public IQueryable<Order> GetSource()
                => from order in _query.For<Facts::Order>()
                   select new Order
                       {
                           Id = order.Id,
                           BeginDistribution = order.BeginDistribution,
                           EndDistributionPlan = order.EndDistributionPlan,
                           IsCommitted = Facts::Order.State.Committed.Contains(order.WorkflowStep)
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

        public sealed class OrderPricePositionAccessor : DataChangesHandler<Order.OrderPricePosition>, IStorageBasedDataObjectAccessor<Order.OrderPricePosition>
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
                        MessageTypeCode.OrderPositionMayCorrespontToActualPrice,
                        MessageTypeCode.OrderPositionMustCorrespontToActualPrice,
                    };

            public IQueryable<Order.OrderPricePosition> GetSource()
                =>
                    from order in _query.For<Facts::Order>() // Чтобы сократить число позиций
                    from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                    from pricePosition in _query.For<Facts::PricePosition>().Where(x => x.Id == orderPosition.PricePositionId)
                    select new Order.OrderPricePosition
                    {
                        OrderId = orderPosition.OrderId,
                        OrderPositionId = orderPosition.Id,
                        PositionId = pricePosition.PositionId,

                        PriceId = pricePosition.PriceId,
                        IsActive = pricePosition.IsActiveNotDeleted,
                    };


            public FindSpecification<Order.OrderPricePosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.OrderPricePosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderCategoryPositionAccessor : DataChangesHandler<Order.OrderCategoryPosition>, IStorageBasedDataObjectAccessor<Order.OrderCategoryPosition>
        {
            private readonly IQuery _query;

            public OrderCategoryPositionAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited,
                    };

            public IQueryable<Order.OrderCategoryPosition> GetSource()
            {
                var result =
                    from order in _query.For<Facts::Order>()
                    from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                    from opa in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.CategoryId.HasValue).Where(x => x.OrderPositionId == orderPosition.Id)
                    from position in _query.For<Facts::Position>().Where(x => x.CategoryCode == Facts::Position.CategoryCodeAdvertisementInCategory).Where(x => x.Id == opa.PositionId) // join для того, чтобы отбросить неподходящие продажи
                    select new Order.OrderCategoryPosition
                    {
                        OrderId = order.Id,
                        OrderPositionAdvertisementId = opa.Id,
                        CategoryId = opa.CategoryId.Value,
                    };

                return result;
            }

            public FindSpecification<Order.OrderCategoryPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.OrderCategoryPosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderThemePositionAccessor : DataChangesHandler<Order.OrderThemePosition>, IStorageBasedDataObjectAccessor<Order.OrderThemePosition>
        {
            private readonly IQuery _query;

            public OrderThemePositionAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.AdvertisementCountPerThemeShouldBeLimited,
                    };

            public IQueryable<Order.OrderThemePosition> GetSource()
            {
                var result =
                    from order in _query.For<Facts::Order>()
                    from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                    from opa in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.ThemeId.HasValue).Where(x => x.OrderPositionId == orderPosition.Id)
                    select new Order.OrderThemePosition
                    {
                        OrderId = order.Id,
                        OrderPositionAdvertisementId = opa.Id,
                        ThemeId = opa.ThemeId.Value,
                    };

                return result;
            }

            public FindSpecification<Order.OrderThemePosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.OrderThemePosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class AmountControlledPositionAccessor : DataChangesHandler<Order.AmountControlledPosition>, IStorageBasedDataObjectAccessor<Order.AmountControlledPosition>
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

            public IQueryable<Order.AmountControlledPosition> GetSource()
                => (from order in _query.For<Facts::Order>() // Чтобы сократить число позиций
                   join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                   join adv in _query.For<Facts::OrderPositionAdvertisement>() on orderPosition.Id equals adv.OrderPositionId
                   join position in _query.For<Facts::Position>().Where(x => !x.IsDeleted && x.IsControlledByAmount) on adv.PositionId equals position.Id
                   select new Order.AmountControlledPosition
                       {
                           OrderId = orderPosition.OrderId,
                           CategoryCode = position.CategoryCode,
                       }).Distinct();

            public FindSpecification<Order.AmountControlledPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.AmountControlledPosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class ActualPriceAccessor : DataChangesHandler<Order.ActualPrice>, IStorageBasedDataObjectAccessor<Order.ActualPrice>
        {
            private readonly IQuery _query;

            public ActualPriceAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderPositionMayCorrespontToActualPrice,
                        MessageTypeCode.OrderPositionMustCorrespontToActualPrice,
                        MessageTypeCode.OrderMustHaveActualPrice,
                    };

            public IQueryable<Order.ActualPrice> GetSource()
            {
                return
                    from order in _query.For<Facts::Order>()
                    let price = _query.For<Facts::Price>()
                                .Where(x => x.OrganizationUnitId == order.DestOrganizationUnitId)
                                .Where(x => x.BeginDate <= order.BeginDistribution)
                                .OrderByDescending(x => x.BeginDate)
                                .FirstOrDefault()
                    select new Order.ActualPrice
                    {
                        OrderId = order.Id,
                        PriceId = price != null ? (long?)price.Id : null
                    };
            }

            public FindSpecification<Order.ActualPrice> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.ActualPrice>(x => aggregateIds.Contains(x.OrderId));
            }
        }
    }
}