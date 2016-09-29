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
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Aggregates
{
    public sealed class OrderAggregateRootActor : EntityActorBase<Order>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IEqualityComparerFactory _equalityComparerFactory;
        private readonly IBulkRepository<Order.LinkedProject> _linkedProjectBulkRepository;
        private readonly IBulkRepository<Order.RequiredAdvertisementMissing> _requiredAdvertisementMissingBulkRepository;
        private readonly IBulkRepository<Order.RequiredLinkedObjectCompositeMissing> _requiredLinkedObjectCompositeMissingBulkRepository;
        private readonly IBulkRepository<Order.AdvertisementDeleted> _advertisementDeletedBulkRepository;
        private readonly IBulkRepository<Order.AdvertisementMustBelongToFirm> _advertisementMustBelongToFirmBulkRepository;
        private readonly IBulkRepository<Order.AdvertisementIsDummy> _advertisementIsDummyBulkRepository;
        private readonly IBulkRepository<Order.OrderAdvertisement> _orderAdvertisementBulkRepository;

        public OrderAggregateRootActor(
            IQuery query,
            IBulkRepository<Order> orderBulkRepository,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Order.LinkedProject> linkedProjectBulkRepository,
            IBulkRepository<Order.RequiredAdvertisementMissing> requiredAdvertisementMissingBulkRepository,
            IBulkRepository<Order.RequiredLinkedObjectCompositeMissing> requiredLinkedObjectCompositeMissingBulkRepository,
            IBulkRepository<Order.AdvertisementDeleted> advertisementDeletedBulkRepository,
            IBulkRepository<Order.AdvertisementMustBelongToFirm> advertisementMustBelongToFirmBulkRepository,
            IBulkRepository<Order.AdvertisementIsDummy> advertisementIsDummyBulkRepository,
            IBulkRepository<Order.OrderAdvertisement> orderAdvertisementBulkRepository)
            : base(query, orderBulkRepository, equalityComparerFactory, new OrderAccessor(query))
        {
            _query = query;
            _equalityComparerFactory = equalityComparerFactory;
            _linkedProjectBulkRepository = linkedProjectBulkRepository;
            _requiredAdvertisementMissingBulkRepository = requiredAdvertisementMissingBulkRepository;
            _requiredLinkedObjectCompositeMissingBulkRepository = requiredLinkedObjectCompositeMissingBulkRepository;
            _advertisementDeletedBulkRepository = advertisementDeletedBulkRepository;
            _advertisementMustBelongToFirmBulkRepository = advertisementMustBelongToFirmBulkRepository;
            _advertisementIsDummyBulkRepository = advertisementIsDummyBulkRepository;
            _orderAdvertisementBulkRepository = orderAdvertisementBulkRepository;
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<Order.LinkedProject>(_query, _linkedProjectBulkRepository, _equalityComparerFactory, new LinkedProjectAccessor(_query)),
                    new ValueObjectActor<Order.RequiredAdvertisementMissing>(_query, _requiredAdvertisementMissingBulkRepository, _equalityComparerFactory, new RequiredAdvertisementMissingAccessor(_query)),
                    new ValueObjectActor<Order.RequiredLinkedObjectCompositeMissing>(_query, _requiredLinkedObjectCompositeMissingBulkRepository, _equalityComparerFactory, new RequiredLinkedObjectCompositeMissingAccessor(_query)),
                    new ValueObjectActor<Order.AdvertisementDeleted>(_query, _advertisementDeletedBulkRepository, _equalityComparerFactory, new AdvertisementDeletedAccessor(_query)),
                    new ValueObjectActor<Order.AdvertisementMustBelongToFirm>(_query, _advertisementMustBelongToFirmBulkRepository, _equalityComparerFactory, new AdvertisementMustBelongToFirmAccessor(_query)),
                    new ValueObjectActor<Order.AdvertisementIsDummy>(_query, _advertisementIsDummyBulkRepository, _equalityComparerFactory, new AdvertisementIsDummyAccessor(_query)),
                    new ValueObjectActor<Order.OrderAdvertisement>(_query, _orderAdvertisementBulkRepository, _equalityComparerFactory, new OrderAdvertisementAccessor(_query)),
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

                           BeginDistributionDate = order.BeginDistributionDate,
                           EndDistributionDatePlan = order.EndDistributionDatePlan,
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

        public sealed class LinkedProjectAccessor : IStorageBasedDataObjectAccessor<Order.LinkedProject>
        {
            private readonly IQuery _query;

            public LinkedProjectAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.LinkedProject> GetSource()
            {
                var linkedOrganizationUnits = _query.For<Facts::Order>().Select(x => new { OrderId = x.Id, OrganizationUnitId = x.DestOrganizationUnitId })
                                                    .Union(_query.For<Facts::Order>().Select(x => new { OrderId = x.Id, OrganizationUnitId = x.SourceOrganizationUnitId }));

                return from linkedOrganizationUnit in linkedOrganizationUnits
                       from project in _query.For<Facts::Project>().Where(x => x.OrganizationUnitId == linkedOrganizationUnit.OrganizationUnitId)
                       select new Order.LinkedProject { OrderId = linkedOrganizationUnit.OrderId, ProjectId = project.Id };
            }

            public FindSpecification<Order.LinkedProject> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.LinkedProject>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class RequiredAdvertisementMissingAccessor : IStorageBasedDataObjectAccessor<Order.RequiredAdvertisementMissing>
        {
            private readonly IQuery _query;

            public RequiredAdvertisementMissingAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.RequiredAdvertisementMissing> GetSource()
            {
                var positionChilds = from position in _query.For<Facts::Position>()
                                     select new
                                     {
                                         PositionId = position.Id,
                                         ChildPositionId = position.ChildPositionId ?? position.Id,
                                     };

                return from order in _query.For<Facts::Order>()
                       join op in _query.For<Facts::OrderPosition>() on order.Id equals op.OrderId
                       join pp in _query.For<Facts::PricePosition>() on op.PricePositionId equals pp.Id
                       join positionChild in positionChilds on pp.PositionId equals positionChild.PositionId
                       join p in _query.For<Facts::Position>() on positionChild.ChildPositionId equals p.Id
                       join template in _query.For<Facts::AdvertisementTemplate>() on p.AdvertisementTemplateId equals template.Id
                       where template.IsAdvertisementRequired // РМ должен быть указан
                       join opa in _query.For<Facts::OrderPositionAdvertisement>() on new { OrderPositionId = op.Id, PositionId = p.Id } equals new { opa.OrderPositionId, opa.PositionId }
                       where opa.AdvertisementId == null // РМ не указан
                       select new Order.RequiredAdvertisementMissing
                           {
                               OrderId = order.Id,
                               OrderPositionId = op.Id,
                               CompositePositionId = pp.PositionId,
                               PositionId = p.Id,
                       };
            }

            public FindSpecification<Order.RequiredAdvertisementMissing> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.RequiredAdvertisementMissing>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class RequiredLinkedObjectCompositeMissingAccessor : IStorageBasedDataObjectAccessor<Order.RequiredLinkedObjectCompositeMissing>
        {
            private readonly IQuery _query;

            public RequiredLinkedObjectCompositeMissingAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.RequiredLinkedObjectCompositeMissing> GetSource()
                => from order in _query.For<Facts::Order>()
                   join op in _query.For<Facts::OrderPosition>() on order.Id equals op.OrderId
                   join pp in _query.For<Facts::PricePosition>() on op.PricePositionId equals pp.Id
                   join position in _query.For<Facts::Position>() on pp.PositionId equals position.Id
                   where !position.IsCompositionOptional // нужен хотя бы один объект привязки
                   join childPosition in _query.For<Facts::Position>() on position.ChildPositionId equals childPosition.Id
                   from opa in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.OrderPositionId == op.Id && x.PositionId == childPosition.Id).DefaultIfEmpty()
                   where opa == null // объект привязки отсутствует
                   select new Order.RequiredLinkedObjectCompositeMissing
                   {
                       OrderId = order.Id,
                       OrderPositionId = op.Id,
                       CompositePositionId = pp.PositionId,
                       PositionId = childPosition.Id,
                   };

            public FindSpecification<Order.RequiredLinkedObjectCompositeMissing> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.RequiredLinkedObjectCompositeMissing>(x => aggregateIds.Contains(x.OrderId));
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
                   from template in _query.For<Facts::AdvertisementTemplate>()
                   where opa.AdvertisementId == template.DummyAdvertisementId // РМ является заглушкой
                   select new Order.AdvertisementIsDummy
                   {
                       OrderId = order.Id,
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

        public sealed class OrderAdvertisementAccessor : IStorageBasedDataObjectAccessor<Order.OrderAdvertisement>
        {
            private readonly IQuery _query;

            public OrderAdvertisementAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.OrderAdvertisement> GetSource()
                => (from order in _query.For<Facts::Order>()
                   join op in _query.For<Facts::OrderPosition>() on order.Id equals op.OrderId
                   join opa in _query.For<Facts::OrderPositionAdvertisement>() on op.Id equals opa.OrderPositionId
                   where opa.AdvertisementId != null
                   select new Order.OrderAdvertisement
                   {
                       OrderId = order.Id,
                       AdvertisementId = opa.AdvertisementId.Value
                   }).Distinct();

            public FindSpecification<Order.OrderAdvertisement> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.OrderAdvertisement>(x => aggregateIds.Contains(x.OrderId));
            }
        }
    }
}