using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Aggregates
{
    public sealed class OrderAggregateRootActor : AggregateRootActor<Order>
    {
        public OrderAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Order> bulkRepository,
            IBulkRepository<Order.MissingOrderPositionAdvertisement> missingOrderPositionAdvertisementBulkRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new OrderAccessor(query), bulkRepository,
                HasValueObject(new MissingOrderPositionAdvertisementAccessor(query), missingOrderPositionAdvertisementBulkRepository));
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
                        MessageTypeCode.OrderPositionAdvertisementMustBeCreated,
                    };

            public IQueryable<Order> GetSource()
                => from order in _query.For<Facts::Order>()
                   from project in _query.For<Facts::Project>().Where(x => x.OrganizationUnitId == order.DestOrganizationUnitId)
                   select new Order
                       {
                           Id = order.Id,

                           BeginDistributionDate = order.BeginDistribution,
                           EndDistributionDatePlan = order.EndDistributionPlan,
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

        public sealed class MissingOrderPositionAdvertisementAccessor : DataChangesHandler<Order.MissingOrderPositionAdvertisement>, IStorageBasedDataObjectAccessor<Order.MissingOrderPositionAdvertisement>
        {
            private readonly IQuery _query;

            public MissingOrderPositionAdvertisementAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderPositionAdvertisementMustBeCreated,
                    };

            public IQueryable<Order.MissingOrderPositionAdvertisement> GetSource()
                => from order in _query.For<Facts::Order>()
                   join op in _query.For<Facts::OrderPosition>() on order.Id equals op.OrderId
                   join pp in _query.For<Facts::PricePosition>().Where(x => x.IsActiveNotDeleted) on op.PricePositionId equals pp.Id
                   join position in _query.For<Facts::Position>().Where(x => !x.IsDeleted && !x.IsCompositionOptional) on pp.PositionId equals position.Id
                   join childPosition in _query.For<Facts::PositionChild>() on position.Id equals childPosition.MasterPositionId
                   from opa in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.OrderPositionId == op.Id && x.PositionId == childPosition.ChildPositionId).DefaultIfEmpty()
                   where opa == null // позиция не продана
                   select new Order.MissingOrderPositionAdvertisement
                   {
                       OrderId = order.Id,
                       OrderPositionId = op.Id,
                       CompositePositionId = pp.PositionId,
                       PositionId = childPosition.ChildPositionId,
                   };

            public FindSpecification<Order.MissingOrderPositionAdvertisement> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.MissingOrderPositionAdvertisement>(x => aggregateIds.Contains(x.OrderId));
            }
        }
    }
}