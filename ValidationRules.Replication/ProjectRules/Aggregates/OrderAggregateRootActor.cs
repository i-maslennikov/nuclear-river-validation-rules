using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.ProjectRules.Aggregates
{
    public sealed class OrderAggregateRootActor : AggregateRootActor<Order>
    {
        public OrderAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Order> bulkRepository,
            IBulkRepository<Order.AddressAdvertisementNonOnTheMap> addressAdvertisementRepository,
            IBulkRepository<Order.CategoryAdvertisement> categoryAdvertisementRepository,
            IBulkRepository<Order.CostPerClickAdvertisement> costPerClickAdvertisementRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new OrderAccessor(query), bulkRepository,
               HasValueObject(new AddressAdvertisementNonOnTheMapAccessor(query), addressAdvertisementRepository),
               HasValueObject(new CategoryAdvertisementAccessor(query), categoryAdvertisementRepository),
               HasValueObject(new CostPerClickAdvertisementAccessor(query), costPerClickAdvertisementRepository));
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
                        MessageTypeCode.FirmAddressMustBeLocatedOnTheMap,
                        MessageTypeCode.OrderMustNotIncludeReleasedPeriod,
                        MessageTypeCode.OrderMustUseCategoriesOnlyAvailableInProject,
                        MessageTypeCode.OrderPositionCostPerClickMustBeSpecified,
                        MessageTypeCode.OrderPositionCostPerClickMustNotBeLessMinimum,
                        MessageTypeCode.OrderPositionSalesModelMustMatchCategorySalesModel,
                        MessageTypeCode.ProjectMustContainCostPerClickMinimumRestriction,
                        MessageTypeCode.ProjectMustContainCostPerClickMinimumRestrictionMass,
                    };

            public IQueryable<Order> GetSource()
                => from order in _query.For<Facts::Order>()
                   from project in _query.For<Facts::Project>().Where(x => x.OrganizationUnitId == order.DestOrganizationUnitId)
                   select new Order
                       {
                           Id = order.Id,
                           Begin = order.BeginDistribution,
                           End = order.EndDistributionPlan, // ?
                           ProjectId = project.Id,
                           IsDraft = order.WorkflowStep == Facts::Order.State.OnRegistration,
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

        public sealed class AddressAdvertisementNonOnTheMapAccessor : DataChangesHandler<Order.AddressAdvertisementNonOnTheMap>, IStorageBasedDataObjectAccessor<Order.AddressAdvertisementNonOnTheMap>
        {
            private readonly IQuery _query;

            public AddressAdvertisementNonOnTheMapAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.FirmAddressMustBeLocatedOnTheMap,
                    };

            public IQueryable<Order.AddressAdvertisementNonOnTheMap> GetSource()
                => (from order in _query.For<Facts::Order>()
                    from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                    from orderPositionAdvertisement in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.FirmAddressId.HasValue).Where(x => x.OrderPositionId == orderPosition.Id)
                    from position in _query.For<Facts::Position>()
                                           .Where(x => !x.IsDeleted && !Facts::Position.CategoryCodesAllowNotLocatedOnTheMap.Contains(x.CategoryCode))
                                           .Where(x => x.Id == orderPositionAdvertisement.PositionId)
                    from firmAddress in _query.For<Facts::FirmAddress>().Where(x => !x.IsLocatedOnTheMap).Where(x => x.Id == orderPositionAdvertisement.FirmAddressId)
                    select new Order.AddressAdvertisementNonOnTheMap
                        {
                            OrderId = order.Id,
                            OrderPositionId = orderPosition.Id,
                            PositionId = position.Id,
                            AddressId = firmAddress.Id,
                        }).Distinct();

        public FindSpecification<Order.AddressAdvertisementNonOnTheMap> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.AddressAdvertisementNonOnTheMap>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class CategoryAdvertisementAccessor : DataChangesHandler<Order.CategoryAdvertisement>, IStorageBasedDataObjectAccessor<Order.CategoryAdvertisement>
        {
            private readonly IQuery _query;

            public CategoryAdvertisementAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderMustUseCategoriesOnlyAvailableInProject,
                        MessageTypeCode.OrderPositionCostPerClickMustBeSpecified,
                        MessageTypeCode.OrderPositionSalesModelMustMatchCategorySalesModel,
                    };

            public IQueryable<Order.CategoryAdvertisement> GetSource()
                => (from order in _query.For<Facts::Order>()
                    from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                    from opa in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id)
                    from position in _query.For<Facts::Position>().Where(x => !x.IsDeleted).Where(x => x.Id == opa.PositionId)
                    from category in _query.For<Facts::Category>().Where(x => x.IsActiveNotDeleted).Where(x => x.Id == opa.CategoryId)
                    where opa.CategoryId.HasValue
                    select new Order.CategoryAdvertisement
                        {
                            OrderId = order.Id,
                            OrderPositionId = orderPosition.Id,
                            PositionId = opa.PositionId,
                            CategoryId = category.Id,
                            SalesModel = position.SalesModel,
                            IsSalesModelRestrictionApplicable = category.L3Id != null && position.PositionsGroup != Facts::Position.PositionsGroupMedia
                    }).Distinct();

            public FindSpecification<Order.CategoryAdvertisement> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.CategoryAdvertisement>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class CostPerClickAdvertisementAccessor : DataChangesHandler<Order.CostPerClickAdvertisement>, IStorageBasedDataObjectAccessor<Order.CostPerClickAdvertisement>
        {
            private readonly IQuery _query;

            public CostPerClickAdvertisementAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderPositionCostPerClickMustBeSpecified,
                        MessageTypeCode.OrderPositionCostPerClickMustNotBeLessMinimum,
                        MessageTypeCode.ProjectMustContainCostPerClickMinimumRestriction,
                        MessageTypeCode.ProjectMustContainCostPerClickMinimumRestrictionMass,
                    };

            public IQueryable<Order.CostPerClickAdvertisement> GetSource()
                => from order in _query.For<Facts::Order>()
                   from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                   from pricePosition in _query.For<Facts::PricePosition>().Where(x => x.Id == orderPosition.PricePositionId)
                   from cpc in _query.For<Facts::OrderPositionCostPerClick>().Where(x => x.OrderPositionId == orderPosition.Id)
                   select new Order.CostPerClickAdvertisement
                       {
                           OrderId = order.Id,
                           OrderPositionId = orderPosition.Id,
                           PositionId = pricePosition.PositionId,
                           CategoryId = cpc.CategoryId,
                           Bid = cpc.Amount,
                       };

            public FindSpecification<Order.CostPerClickAdvertisement> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.CostPerClickAdvertisement>(x => aggregateIds.Contains(x.OrderId));
            }
        }
    }
}
