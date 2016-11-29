using System;
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
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Aggregates
{
    public sealed class OrderAggregateRootActor : EntityActorBase<Order>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IEqualityComparerFactory _equalityComparerFactory;
        private readonly IBulkRepository<Order.MissingAdvertisementReference> _missingAdvertisementReferenceBulkRepository;
        private readonly IBulkRepository<Order.MissingOrderPositionAdvertisement> _missingOrderPositionAdvertisementBulkRepository;
        private readonly IBulkRepository<Order.AdvertisementDeleted> _advertisementDeletedBulkRepository;
        private readonly IBulkRepository<Order.AdvertisementMustBelongToFirm> _advertisementMustBelongToFirmBulkRepository;
        private readonly IBulkRepository<Order.AdvertisementIsDummy> _advertisementIsDummyBulkRepository;
        private readonly IBulkRepository<Order.CouponDistributionPeriod> _couponDistributionPeriodRepository;
        private readonly IBulkRepository<Order.OrderPositionAdvertisement> _orderPositionAdvertisementBulkRepository;

        public OrderAggregateRootActor(
            IQuery query,
            IBulkRepository<Order> orderBulkRepository,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Order.MissingAdvertisementReference> missingAdvertisementReferenceBulkRepository,
            IBulkRepository<Order.MissingOrderPositionAdvertisement> missingOrderPositionAdvertisementBulkRepository,
            IBulkRepository<Order.AdvertisementDeleted> advertisementDeletedBulkRepository,
            IBulkRepository<Order.AdvertisementMustBelongToFirm> advertisementMustBelongToFirmBulkRepository,
            IBulkRepository<Order.AdvertisementIsDummy> advertisementIsDummyBulkRepository,
            IBulkRepository<Order.CouponDistributionPeriod> couponDistributionPeriodRepository,
            IBulkRepository<Order.OrderPositionAdvertisement> orderPositionAdvertisementBulkRepository)
            : base(query, orderBulkRepository, equalityComparerFactory, new OrderAccessor(query))
        {
            _query = query;
            _equalityComparerFactory = equalityComparerFactory;
            _missingAdvertisementReferenceBulkRepository = missingAdvertisementReferenceBulkRepository;
            _missingOrderPositionAdvertisementBulkRepository = missingOrderPositionAdvertisementBulkRepository;
            _advertisementDeletedBulkRepository = advertisementDeletedBulkRepository;
            _advertisementMustBelongToFirmBulkRepository = advertisementMustBelongToFirmBulkRepository;
            _advertisementIsDummyBulkRepository = advertisementIsDummyBulkRepository;
            _couponDistributionPeriodRepository = couponDistributionPeriodRepository;
            _orderPositionAdvertisementBulkRepository = orderPositionAdvertisementBulkRepository;
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<Order.MissingAdvertisementReference>(_query, _missingAdvertisementReferenceBulkRepository, _equalityComparerFactory, new MissingAdvertisementReferenceAccessor(_query)),
                    new ValueObjectActor<Order.MissingOrderPositionAdvertisement>(_query, _missingOrderPositionAdvertisementBulkRepository, _equalityComparerFactory, new MissingOrderPositionAdvertisementAccessor(_query)),
                    new ValueObjectActor<Order.AdvertisementDeleted>(_query, _advertisementDeletedBulkRepository, _equalityComparerFactory, new AdvertisementDeletedAccessor(_query)),
                    new ValueObjectActor<Order.AdvertisementMustBelongToFirm>(_query, _advertisementMustBelongToFirmBulkRepository, _equalityComparerFactory, new AdvertisementMustBelongToFirmAccessor(_query)),
                    new ValueObjectActor<Order.AdvertisementIsDummy>(_query, _advertisementIsDummyBulkRepository, _equalityComparerFactory, new AdvertisementIsDummyAccessor(_query)),
                    new ValueObjectActor<Order.CouponDistributionPeriod>(_query, _couponDistributionPeriodRepository, _equalityComparerFactory, new CouponDistributionPeriodAccessor(_query)),
                    new ValueObjectActor<Order.OrderPositionAdvertisement>(_query, _orderPositionAdvertisementBulkRepository, _equalityComparerFactory, new OrderPositionAdvertisementAccessor(_query)),
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
                   let require = (from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                                  from opa in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id)
                                  from a in _query.For<Facts::Advertisement>().Where(x => !x.IsDeleted && x.Id == opa.AdvertisementId)
                                  from at in _query.For<Facts::AdvertisementTemplate>().Where(x => x.Id == a.AdvertisementTemplateId)
                                  select at.IsAllowedToWhiteList).Any(x => x)
                   let provide = (from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                                  from opa in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id)
                                  from a in _query.For<Facts::Advertisement>().Where(x => !x.IsDeleted && x.Id == opa.AdvertisementId)
                                  select a.IsSelectedToWhiteList).Any(x => x)
                   select new Order
                       {
                           Id = order.Id,
                           Number = order.Number,

                           BeginDistributionDate = order.BeginDistribution,
                           EndDistributionDatePlan = order.EndDistributionPlan,
                           EndDistributionDateFact = order.EndDistributionFact,
                           ProjectId = project.Id,
                           FirmId = order.FirmId,
                           RequireWhiteListAdvertisement = require,
                           ProvideWhiteListAdvertisement = provide,
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

        public sealed class MissingAdvertisementReferenceAccessor : IStorageBasedDataObjectAccessor<Order.MissingAdvertisementReference>
        {
            private readonly IQuery _query;

            public MissingAdvertisementReferenceAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.MissingAdvertisementReference> GetSource()
            {
                var positionChilds = from position in _query.For<Facts::Position>().Where(x => !x.IsDeleted)
                                     from child in _query.For<Facts::PositionChild>().Where(x => x.MasterPositionId == position.Id).DefaultIfEmpty()
                                     select new
                                     {
                                         PositionId = position.Id,
                                         ChildPositionId = child != null ? child.ChildPositionId : position.Id
                                     };

                return from order in _query.For<Facts::Order>()
                       join op in _query.For<Facts::OrderPosition>() on order.Id equals op.OrderId
                       join pp in _query.For<Facts::PricePosition>().Where(x => x.IsActiveNotDeleted) on op.PricePositionId equals pp.Id
                       join positionChild in positionChilds on pp.PositionId equals positionChild.PositionId
                       join p in _query.For<Facts::Position>().Where(x => !x.IsDeleted) on positionChild.ChildPositionId equals p.Id
                       join template in _query.For<Facts::AdvertisementTemplate>() on p.AdvertisementTemplateId equals template.Id
                       where template.IsAdvertisementRequired // РМ должен быть указан
                       join opa in _query.For<Facts::OrderPositionAdvertisement>() on new { OrderPositionId = op.Id, PositionId = p.Id } equals new { opa.OrderPositionId, opa.PositionId }
                       where opa.AdvertisementId == null // РМ не указан
                       select new Order.MissingAdvertisementReference
                           {
                               OrderId = order.Id,
                               OrderPositionId = op.Id,
                               CompositePositionId = pp.PositionId,
                               PositionId = p.Id,
                       };
            }

            public FindSpecification<Order.MissingAdvertisementReference> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.MissingAdvertisementReference>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class MissingOrderPositionAdvertisementAccessor : IStorageBasedDataObjectAccessor<Order.MissingOrderPositionAdvertisement>
        {
            private readonly IQuery _query;

            public MissingOrderPositionAdvertisementAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.MissingOrderPositionAdvertisement> GetSource()
                => from order in _query.For<Facts::Order>()
                   join op in _query.For<Facts::OrderPosition>() on order.Id equals op.OrderId
                   join pp in _query.For<Facts::PricePosition>().Where(x => x.IsActiveNotDeleted) on op.PricePositionId equals pp.Id
                   join position in _query.For<Facts::Position>().Where(x => !x.IsDeleted) on pp.PositionId equals position.Id
                   where !position.IsCompositionOptional // нужен хотя бы один объект привязки
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
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.MissingOrderPositionAdvertisement>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class AdvertisementDeletedAccessor : IStorageBasedDataObjectAccessor<Order.AdvertisementDeleted>
        {
            private readonly IQuery _query;

            public AdvertisementDeletedAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.AdvertisementDeleted> GetSource()
                => from order in _query.For<Facts::Order>()
                   join op in _query.For<Facts::OrderPosition>() on order.Id equals op.OrderId
                   join opa in _query.For<Facts::OrderPositionAdvertisement>() on op.Id equals opa.OrderPositionId
                   join advertisement in _query.For<Facts::Advertisement>() on opa.AdvertisementId equals advertisement.Id
                   where advertisement.IsDeleted // РМ удалён
                   select new Order.AdvertisementDeleted
                   {
                       OrderId = order.Id,
                       OrderPositionId = op.Id,
                       PositionId = opa.PositionId,
                       AdvertisementId = advertisement.Id,
                       AdvertisementName = advertisement.Name,
                   };

            public FindSpecification<Order.AdvertisementDeleted> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.AdvertisementDeleted>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class AdvertisementMustBelongToFirmAccessor : IStorageBasedDataObjectAccessor<Order.AdvertisementMustBelongToFirm>
        {
            private readonly IQuery _query;

            public AdvertisementMustBelongToFirmAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.AdvertisementMustBelongToFirm> GetSource()
                => from order in _query.For<Facts::Order>()
                   join firm in _query.For<Facts::Firm>() on order.FirmId equals firm.Id
                   join op in _query.For<Facts::OrderPosition>() on order.Id equals op.OrderId
                   join opa in _query.For<Facts::OrderPositionAdvertisement>() on op.Id equals opa.OrderPositionId
                   join advertisement in _query.For<Facts::Advertisement>().Where(x => !x.IsDeleted) on opa.AdvertisementId equals advertisement.Id
                   where advertisement.FirmId != order.FirmId // РМ не принадлежит фирме заказа
                   select new Order.AdvertisementMustBelongToFirm
                   {
                       OrderId = order.Id,
                       OrderPositionId = op.Id,
                       PositionId = opa.PositionId,
                       AdvertisementId = advertisement.Id,
                       FirmId = firm.Id,
                   };

            public FindSpecification<Order.AdvertisementMustBelongToFirm> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.AdvertisementMustBelongToFirm>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class AdvertisementIsDummyAccessor : IStorageBasedDataObjectAccessor<Order.AdvertisementIsDummy>
        {
            private readonly IQuery _query;

            public AdvertisementIsDummyAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.AdvertisementIsDummy> GetSource()
                => from order in _query.For<Facts::Order>()
                   join op in _query.For<Facts::OrderPosition>() on order.Id equals op.OrderId
                   join opa in _query.For<Facts::OrderPositionAdvertisement>() on op.Id equals opa.OrderPositionId
                   join advertisement in _query.For<Facts::Advertisement>() on opa.AdvertisementId equals advertisement.Id
                   join template in _query.For<Facts::AdvertisementTemplate>() on advertisement.AdvertisementTemplateId equals template.Id
                   where advertisement.Id == template.DummyAdvertisementId // РМ является заглушкой
                   select new Order.AdvertisementIsDummy
                   {
                       OrderId = order.Id,
                       OrderPositionId = op.Id,
                       PositionId = opa.PositionId,
                   };

            public FindSpecification<Order.AdvertisementIsDummy> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.AdvertisementIsDummy>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class CouponDistributionPeriodAccessor : IStorageBasedDataObjectAccessor<Order.CouponDistributionPeriod>
        {
            private const int CouponPositionCategoryCode = 14;

            private readonly IQuery _query;

            public CouponDistributionPeriodAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.CouponDistributionPeriod> GetSource()
                => from order in GetOrdersFact().Union(GetOrdersPlan())
                   join op in _query.For<Facts::OrderPosition>() on order.Id equals op.OrderId
                   join opa in _query.For<Facts::OrderPositionAdvertisement>() on op.Id equals opa.OrderPositionId
                   join position in _query.For<Facts::Position>().Where(x => !x.IsDeleted) on opa.PositionId equals position.Id
                   join advertisement in _query.For<Facts::Advertisement>() on opa.AdvertisementId equals advertisement.Id
                   join template in _query.For<Facts::AdvertisementTemplate>() on advertisement.AdvertisementTemplateId equals template.Id
                   where position.CategoryCode == CouponPositionCategoryCode // выгодные покупки с 2ГИС
                   where opa.AdvertisementId != template.DummyAdvertisementId // РМ не является заглушкой
                   select new Order.CouponDistributionPeriod
                       {
                           OrderId = order.Id,
                           OrderPositionId = op.Id,
                           PositionId = position.Id,
                           AdvertisementId = advertisement.Id,
                           Begin = order.Begin,
                           End = order.End,
                           Scope = order.Scope
                   };

            private IQueryable<OrderDto> GetOrdersFact()
                => _query.For<Facts::Order>()
                         .Select(x => new OrderDto { Id = x.Id, Begin = x.BeginDistribution, End = x.EndDistributionFact, Scope = Scope.Compute(x.WorkflowStep, x.Id) });

            private IQueryable<OrderDto> GetOrdersPlan()
                => _query.For<Facts::Order>()
                         .Where(x => x.EndDistributionFact != x.EndDistributionPlan)
                         .Select(x => new OrderDto { Id = x.Id, Begin = x.EndDistributionFact, End = x.EndDistributionPlan, Scope = x.Id });

            public FindSpecification<Order.CouponDistributionPeriod> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.CouponDistributionPeriod>(x => aggregateIds.Contains(x.OrderId));
            }

            private sealed class OrderDto
            {
                public long Id { get; set; }
                public DateTime Begin { get; set; }
                public DateTime End { get; set; }
                public long Scope { get; set; }
            }
        }

        public sealed class OrderPositionAdvertisementAccessor : IStorageBasedDataObjectAccessor<Order.OrderPositionAdvertisement>
        {
            private readonly IQuery _query;

            public OrderPositionAdvertisementAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.OrderPositionAdvertisement> GetSource()
                => from order in _query.For<Facts::Order>()
                   from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                   from opa in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id)
                   where opa.AdvertisementId != null
                   select new Order.OrderPositionAdvertisement
                   {
                       OrderId = order.Id,
                       OrderPositionId = orderPosition.Id,
                       PositionId = opa.PositionId,
                       AdvertisementId = opa.AdvertisementId.Value
                   };

            public FindSpecification<Order.OrderPositionAdvertisement> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.OrderPositionAdvertisement>(x => aggregateIds.Contains(x.OrderId));
            }
        }
    }
}