using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Order = NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates.Order;

namespace NuClear.ValidationRules.Replication.FirmRules.Aggregates
{
    public sealed class OrderAggregateRootActor : AggregateRootActor<Order>
    {
        public OrderAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Order> bulkRepository,
            IBulkRepository<Order.FirmOrganiationUnitMismatch> invalidFirmRepository,
            IBulkRepository<Order.InvalidFirm> orderInvalidFirmRepository,
            IBulkRepository<Order.PartnerPosition> premiumProfilePositionRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new OrderAccessor(query),
                          bulkRepository,
                          HasValueObject(new OrderFirmOrganiationUnitMismatchAccessor(query), invalidFirmRepository),
                          HasValueObject(new OrderInvalidFirmAccessor(query), orderInvalidFirmRepository),
                          HasValueObject(new PartnerPositionAccessor(query), premiumProfilePositionRepository));
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
                        MessageTypeCode.FirmAndOrderShouldBelongTheSameOrganizationUnit,
                        MessageTypeCode.FirmShouldHaveLimitedCategoryCount,
                        MessageTypeCode.LinkedFirmShouldBeValid,

                        MessageTypeCode.FirmAddressMustNotHaveMultiplePremiumPartnerAdvertisement,
                        MessageTypeCode.FirmAddressShouldNotHaveMultiplePartnerAdvertisement,
                        MessageTypeCode.PremiumPartnerAdvertisementMustNotBeSoldToAdvertiser,
                        MessageTypeCode.PartnerAdvertisementShouldNotBeSoldToAdvertiser,
                    };

            public IQueryable<Order> GetSource()
                => from order in _query.For<Facts::Order>()
                   select new Order
                       {
                           Id = order.Id,
                           FirmId = order.FirmId,
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

        public sealed class OrderFirmOrganiationUnitMismatchAccessor : DataChangesHandler<Order.FirmOrganiationUnitMismatch>, IStorageBasedDataObjectAccessor<Order.FirmOrganiationUnitMismatch>
        {
            private readonly IQuery _query;

            public OrderFirmOrganiationUnitMismatchAccessor(IQuery query) :base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.FirmAndOrderShouldBelongTheSameOrganizationUnit,
                    };

            public IQueryable<Order.FirmOrganiationUnitMismatch> GetSource()
                => from order in _query.For<Facts::Order>()
                   from firm in _query.For<Facts::Firm>().Where(x => x.IsActive && !x.IsDeleted && !x.IsClosedForAscertainment).Where(x => x.Id == order.FirmId)
                   where order.DestOrganizationUnitId != firm.OrganizationUnitId
                   select new Order.FirmOrganiationUnitMismatch { OrderId = order.Id };

            public FindSpecification<Order.FirmOrganiationUnitMismatch> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.FirmOrganiationUnitMismatch>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class PartnerPositionAccessor : DataChangesHandler<Order.PartnerPosition>, IStorageBasedDataObjectAccessor<Order.PartnerPosition>
        {
            private readonly IQuery _query;

            public PartnerPositionAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.FirmAddressMustNotHaveMultiplePremiumPartnerAdvertisement,
                        MessageTypeCode.FirmAddressShouldNotHaveMultiplePartnerAdvertisement,
                        MessageTypeCode.PremiumPartnerAdvertisementMustNotBeSoldToAdvertiser,
                        MessageTypeCode.PartnerAdvertisementShouldNotBeSoldToAdvertiser,
                    };

            public IQueryable<Order.PartnerPosition> GetSource()
            {
                var ordersWithPremium =
                    from position in _query.For<Facts::Position>().Where(x => x.CategoryCode == Facts::Position.CategoryCodePremiumPartnerAdvertising)
                    from opa in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.PositionId == position.Id)
                    from op in _query.For<Facts::OrderPosition>().Where(x => x.Id == opa.OrderPositionId)
                    select op.OrderId;

                var addressPositions =
                    from position in _query.For<Facts::Position>().Where(x => x.CategoryCode == Facts::Position.CategoryCodePartnerAdvertisingAddress)
                    from opa in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.FirmAddressId.HasValue).Where(x => x.PositionId == position.Id)
                    from op in _query.For<Facts::OrderPosition>().Where(x => x.Id == opa.OrderPositionId)
                    from fa in _query.For<Facts::FirmAddress>().Where(x => x.Id == opa.FirmAddressId.Value)
                    select new Order.PartnerPosition
                    {
                        OrderId = op.OrderId,
                        DestinationFirmAddressId = opa.FirmAddressId.Value,
                        DestinationFirmId = fa.FirmId,
                        IsPremium = ordersWithPremium.Any(x => x == op.OrderId)
                    };

                return addressPositions;
            }

            public FindSpecification<Order.PartnerPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.PartnerPosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderInvalidFirmAccessor : DataChangesHandler<Order.InvalidFirm>, IStorageBasedDataObjectAccessor<Order.InvalidFirm>
        {
            private readonly IQuery _query;

            public OrderInvalidFirmAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.LinkedFirmShouldBeValid,
                    };

            public IQueryable<Order.InvalidFirm> GetSource()
                => from order in _query.For<Facts::Order>()
                   from firm in _query.For<Facts::Firm>().Where(x => !x.IsActive || x.IsDeleted || x.IsClosedForAscertainment).Where(x => x.Id == order.FirmId)
                   let state = firm.IsDeleted ? InvalidFirmState.Deleted
                                   : !firm.IsActive ? InvalidFirmState.ClosedForever
                                   : firm.IsClosedForAscertainment ? InvalidFirmState.ClosedForAscertainment
                                   : InvalidFirmState.NotSet
                   select new Order.InvalidFirm
                   {
                       FirmId = firm.Id,
                       OrderId = order.Id,
                       State = state,
                   };

            public FindSpecification<Order.InvalidFirm> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.InvalidFirm>(x => aggregateIds.Contains(x.OrderId));
            }
        }
    }
}
