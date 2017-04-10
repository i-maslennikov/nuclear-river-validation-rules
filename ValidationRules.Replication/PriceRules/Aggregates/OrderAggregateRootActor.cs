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
            IBulkRepository<Order.OrderPosition> orderPositionBulkRepository,
            IBulkRepository<Order.OrderPricePosition> orderPricePositionBulkRepository,
            IBulkRepository<Order.AmountControlledPosition> amountControlledPositionBulkRepository,
            IBulkRepository<Order.ActualPrice> actualPriceBulkRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new OrderAccessor(query), bulkRepository,
                HasValueObject(new OrderPositionAccessor(query), orderPositionBulkRepository),
                HasValueObject(new OrderPricePositionAccessor(query), orderPricePositionBulkRepository),
                HasValueObject(new AmountControlledPositionAccessor(query), amountControlledPositionBulkRepository),
                HasValueObject(new ActualPriceBulkRepositoryAccessor(query), actualPriceBulkRepository));
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
                        MessageTypeCode.OrderPositionsShouldCorrespontToActualPrice,
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

        public sealed class OrderPositionAccessor : DataChangesHandler<Order.OrderPosition>, IStorageBasedDataObjectAccessor<Order.OrderPosition>
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
                    };

            public IQueryable<Order.OrderPosition> GetSource()
            {
                var opas = from order in _query.For<Facts::Order>() // Чтобы сократить число позиций
                           join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                           join pricePosition in _query.For<Facts::PricePosition>().Where(x => x.IsActiveNotDeleted) on orderPosition.PricePositionId equals pricePosition.Id
                           join opa in _query.For<Facts::OrderPositionAdvertisement>() on orderPosition.Id equals opa.OrderPositionId
                           select new Order.OrderPosition
                           {
                               OrderId = orderPosition.OrderId,
                               ItemPositionId = opa.PositionId,

                               CategoryId = opa.CategoryId,
                               ThemeId = opa.ThemeId,
                           };

                var pkgs = from order in _query.For<Facts::Order>() // Чтобы сократить число позиций
                           join orderPosition in _query.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                           join pricePosition in _query.For<Facts::PricePosition>().Where(x => x.IsActiveNotDeleted) on orderPosition.PricePositionId equals pricePosition.Id
                           join opa in _query.For<Facts::OrderPositionAdvertisement>() on orderPosition.Id equals opa.OrderPositionId
                           join position in _query.For<Facts::Position>().Where(x => !x.IsDeleted).Where(x => x.IsComposite) on pricePosition.PositionId equals position.Id
                           select new Order.OrderPosition
                           {
                               OrderId = orderPosition.OrderId,
                               ItemPositionId = pricePosition.PositionId,

                               CategoryId = opa.CategoryId,
                               ThemeId = opa.ThemeId,
                           };

                return pkgs.Union(opas);
            }

            public FindSpecification<Order.OrderPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.OrderPosition>(x => aggregateIds.Contains(x.OrderId));
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

        public sealed class ActualPriceBulkRepositoryAccessor : DataChangesHandler<Order.ActualPrice>, IStorageBasedDataObjectAccessor<Order.ActualPrice>
        {
            private readonly IQuery _query;

            public ActualPriceBulkRepositoryAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderPositionMayCorrespontToActualPrice,
                        MessageTypeCode.OrderPositionMustCorrespontToActualPrice,
                        MessageTypeCode.OrderPositionsShouldCorrespontToActualPrice,
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