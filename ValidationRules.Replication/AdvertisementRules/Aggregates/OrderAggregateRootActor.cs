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
        private readonly IBulkRepository<Order.RequiredAdvertisementMissing> _requiredAdvertisementMissingBulkRepository;
        private readonly IBulkRepository<Order.RequiredAdvertisementCompositeMissing> _requiredAdvertisementCompositeMissingBulkRepository;
        private readonly IBulkRepository<Order.RequiredLinkedObjectCompositeMissing> _requiredLinkedObjectCompositeMissingBulkRepository;
        private readonly IBulkRepository<Order.AdvertisementDeleted> _advertisementDeletedBulkRepository;
        private readonly IBulkRepository<Order.AdvertisementMustBelongToFirm> _advertisementMustBelongToFirmBulkRepository;
        private readonly IBulkRepository<Order.AdvertisementIsDummy> _advertisementIsDummyBulkRepository;
        private readonly IBulkRepository<Order.OrderAdvertisement> _orderAdvertisementBulkRepository;
        private readonly IBulkRepository<Order.WhiteListNotExist> _whiteListNotExistBulkRepository;
        private readonly IBulkRepository<Order.WhiteListExist> _whiteListExistBulkRepository;

        public OrderAggregateRootActor(
            IQuery query,
            IBulkRepository<Order> orderBulkRepository,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Order.RequiredAdvertisementMissing> requiredAdvertisementMissingBulkRepository,
            IBulkRepository<Order.RequiredAdvertisementCompositeMissing> requiredAdvertisementCompositeMissingBulkRepository,
            IBulkRepository<Order.RequiredLinkedObjectCompositeMissing> requiredLinkedObjectCompositeMissingBulkRepository,
            IBulkRepository<Order.AdvertisementDeleted> advertisementDeletedBulkRepository,
            IBulkRepository<Order.AdvertisementMustBelongToFirm> advertisementMustBelongToFirmBulkRepository,
            IBulkRepository<Order.AdvertisementIsDummy> advertisementIsDummyBulkRepository,
            IBulkRepository<Order.OrderAdvertisement> orderAdvertisementBulkRepository,
            IBulkRepository<Order.WhiteListNotExist> whiteListNotExistBulkRepository,
            IBulkRepository<Order.WhiteListExist> whiteListExistBulkRepository)
            : base(query, orderBulkRepository, equalityComparerFactory, new OrderAccessor(query))
        {
            _query = query;
            _equalityComparerFactory = equalityComparerFactory;
            _requiredAdvertisementMissingBulkRepository = requiredAdvertisementMissingBulkRepository;
            _requiredAdvertisementCompositeMissingBulkRepository = requiredAdvertisementCompositeMissingBulkRepository;
            _requiredLinkedObjectCompositeMissingBulkRepository = requiredLinkedObjectCompositeMissingBulkRepository;
            _advertisementDeletedBulkRepository = advertisementDeletedBulkRepository;
            _advertisementMustBelongToFirmBulkRepository = advertisementMustBelongToFirmBulkRepository;
            _advertisementIsDummyBulkRepository = advertisementIsDummyBulkRepository;
            _orderAdvertisementBulkRepository = orderAdvertisementBulkRepository;
            _whiteListNotExistBulkRepository = whiteListNotExistBulkRepository;
            _whiteListExistBulkRepository = whiteListExistBulkRepository;
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<Order.RequiredAdvertisementMissing>(_query, _requiredAdvertisementMissingBulkRepository, _equalityComparerFactory, new RequiredAdvertisementMissingAccessor(_query)),
                    new ValueObjectActor<Order.RequiredAdvertisementCompositeMissing>(_query, _requiredAdvertisementCompositeMissingBulkRepository, _equalityComparerFactory, new RequiredAdvertisementCompositeMissingAccessor(_query)),
                    new ValueObjectActor<Order.RequiredLinkedObjectCompositeMissing>(_query, _requiredLinkedObjectCompositeMissingBulkRepository, _equalityComparerFactory, new RequiredLinkedObjectCompositeMissingAccessor(_query)),
                    new ValueObjectActor<Order.AdvertisementDeleted>(_query, _advertisementDeletedBulkRepository, _equalityComparerFactory, new AdvertisementDeletedAccessor(_query)),
                    new ValueObjectActor<Order.AdvertisementMustBelongToFirm>(_query, _advertisementMustBelongToFirmBulkRepository, _equalityComparerFactory, new AdvertisementMustBelongToFirmAccessor(_query)),
                    new ValueObjectActor<Order.AdvertisementIsDummy>(_query, _advertisementIsDummyBulkRepository, _equalityComparerFactory, new AdvertisementIsDummyAccessor(_query)),
                    new ValueObjectActor<Order.OrderAdvertisement>(_query, _orderAdvertisementBulkRepository, _equalityComparerFactory, new OrderAdvertisementAccessor(_query)),
                    new ValueObjectActor<Order.WhiteListNotExist>(_query, _whiteListNotExistBulkRepository, _equalityComparerFactory, new WhiteListNotExistAccessor(_query)),
                    new ValueObjectActor<Order.WhiteListExist>(_query, _whiteListExistBulkRepository, _equalityComparerFactory, new WhiteListExistAccessor(_query)),
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

                           BeginDistributionDate = order.BeginDistributionDate,
                           EndDistributionDatePlan = order.EndDistributionDatePlan,
                           ProjectId = project.Id,
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

        public sealed class RequiredAdvertisementMissingAccessor : IStorageBasedDataObjectAccessor<Order.RequiredAdvertisementMissing>
        {
            private readonly IQuery _query;

            public RequiredAdvertisementMissingAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.RequiredAdvertisementMissing> GetSource()
            {
                // только простые позиции
                var simplePositions = from position in _query.For<Facts::Position>()
                                      where position.ChildPositionId == null
                                      select position;

                return from order in _query.For<Facts::Order>()
                       join op in _query.For<Facts::OrderPosition>() on order.Id equals op.OrderId
                       join pp in _query.For<Facts::PricePosition>() on op.PricePositionId equals pp.Id
                       join simplePosition in simplePositions on pp.PositionId equals simplePosition.Id
                       join template in _query.For<Facts::AdvertisementTemplate>() on simplePosition.AdvertisementTemplateId equals template.Id
                       where template.IsAdvertisementRequired // РМ должен быть указан
                       join opa in _query.For<Facts::OrderPositionAdvertisement>() on new { OrderPositionId = op.Id, PositionId = simplePosition.Id } equals new { opa.OrderPositionId, opa.PositionId }
                       where opa.AdvertisementId == null // РМ не указан
                       select new Order.RequiredAdvertisementMissing
                           {
                               OrderId = order.Id,
                               OrderPositionId = op.Id,
                               PositionId = simplePosition.Id,
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

        public sealed class RequiredAdvertisementCompositeMissingAccessor : IStorageBasedDataObjectAccessor<Order.RequiredAdvertisementCompositeMissing>
        {
            private readonly IQuery _query;

            public RequiredAdvertisementCompositeMissingAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.RequiredAdvertisementCompositeMissing> GetSource()
                => from order in _query.For<Facts::Order>()
                   join op in _query.For<Facts::OrderPosition>() on order.Id equals op.OrderId
                   join pp in _query.For<Facts::PricePosition>() on op.PricePositionId equals pp.Id
                   join position in _query.For<Facts::Position>() on pp.PositionId equals position.Id
                   join childPosition in _query.For<Facts::Position>() on position.ChildPositionId equals childPosition.Id
                   join template in _query.For<Facts::AdvertisementTemplate>() on childPosition.AdvertisementTemplateId equals template.Id
                   where template.IsAdvertisementRequired // РМ должен быть указан
                   join opa in _query.For<Facts::OrderPositionAdvertisement>() on new { OrderPositionId = op.Id, PositionId = childPosition.Id } equals new { opa.OrderPositionId, opa.PositionId}
                   where opa.AdvertisementId == null // РМ не указан
                   select new Order.RequiredAdvertisementCompositeMissing
                   {
                       OrderId = order.Id,
                       OrderPositionId = op.Id,
                       CompositePositionId = pp.PositionId,
                       PositionId = childPosition.Id,
                   };

            public FindSpecification<Order.RequiredAdvertisementCompositeMissing> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.RequiredAdvertisementCompositeMissing>(x => aggregateIds.Contains(x.OrderId));
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
                   join advertisement in _query.For<Facts::Advertisement>() on opa.AdvertisementId equals advertisement.Id
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

        public sealed class WhiteListNotExistAccessor : IStorageBasedDataObjectAccessor<Order.WhiteListNotExist>
        {
            private readonly IQuery _query;

            public WhiteListNotExistAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.WhiteListNotExist> GetSource()
            {
                var positionChilds = from position in _query.For<Facts::Position>()
                                    select new
                                    {
                                        PositionId = position.Id,
                                        ChildPositionId = position.ChildPositionId ?? position.Id,
                                    };

                var ordersAllowed = from order in _query.For<Facts::Order>()
                                    join op in _query.For<Facts::OrderPosition>() on order.Id equals op.OrderId
                                    join pp in _query.For<Facts::PricePosition>() on op.PricePositionId equals pp.Id
                                    join positionChild in positionChilds on pp.PositionId equals positionChild.PositionId
                                    join p in _query.For<Facts::Position>() on positionChild.ChildPositionId equals p.Id
                                    join template in _query.For<Facts::AdvertisementTemplate>() on p.AdvertisementTemplateId equals template.Id
                                    where template.IsAllowedToWhiteList // шаблон может быть выбран в белый список
                                    select order;

                var ordersSelected = from order in _query.For<Facts::Order>()
                                    join op in _query.For<Facts::OrderPosition>() on order.Id equals op.OrderId
                                    join opa in _query.For<Facts::OrderPositionAdvertisement>() on op.Id equals opa.OrderPositionId
                                    join advertisement in _query.For<Facts::Advertisement>() on opa.AdvertisementId equals advertisement.Id
                                    where advertisement.IsSelectedToWhiteList // РМ выбран в белый список
                                    select order;

                var whiteListNotExist = (from orderAllowed in ordersAllowed
                                    from orderSelected in ordersSelected.Where(x => x.FirmId == orderAllowed.FirmId).DefaultIfEmpty()
                                    where orderSelected == null
                                    select new Order.WhiteListNotExist
                                    {
                                        OrderId = orderAllowed.Id,
                                        FirmId = orderAllowed.FirmId,
                                    }).Distinct();

                return whiteListNotExist;
            }

            public FindSpecification<Order.WhiteListNotExist> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.WhiteListNotExist>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class WhiteListExistAccessor : IStorageBasedDataObjectAccessor<Order.WhiteListExist>
        {
            private readonly IQuery _query;

            public WhiteListExistAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.WhiteListExist> GetSource()
            {
                var positionChilds = from position in _query.For<Facts::Position>()
                                     select new
                                     {
                                         PositionId = position.Id,
                                         ChildPositionId = position.ChildPositionId ?? position.Id,
                                     };

                var ordersAllowed = from order in _query.For<Facts::Order>()
                                    join op in _query.For<Facts::OrderPosition>() on order.Id equals op.OrderId
                                    join pp in _query.For<Facts::PricePosition>() on op.PricePositionId equals pp.Id
                                    join positionChild in positionChilds on pp.PositionId equals positionChild.PositionId
                                    join p in _query.For<Facts::Position>() on positionChild.ChildPositionId equals p.Id
                                    join template in _query.For<Facts::AdvertisementTemplate>() on p.AdvertisementTemplateId equals template.Id
                                    where template.IsAllowedToWhiteList // шаблон может быть выбран в белый список
                                    select order;

                var ordersSelected = from order in _query.For<Facts::Order>()
                                     join op in _query.For<Facts::OrderPosition>() on order.Id equals op.OrderId
                                     join opa in _query.For<Facts::OrderPositionAdvertisement>() on op.Id equals opa.OrderPositionId
                                     join advertisement in _query.For<Facts::Advertisement>() on opa.AdvertisementId equals advertisement.Id
                                     where advertisement.IsSelectedToWhiteList // РМ выбран в белый список
                                     select new { order, advertisement };

                var whiteListExist = (from orderAllowed in ordersAllowed
                                       join orderSelected in ordersSelected on orderAllowed.FirmId equals orderSelected.order.FirmId
                                       select new Order.WhiteListExist
                                       {
                                           OrderId = orderAllowed.Id,
                                           FirmId = orderAllowed.FirmId,
                                           AdvertisementId = orderSelected.advertisement.Id
                                       }).Distinct();

                return whiteListExist;
            }

            public FindSpecification<Order.WhiteListExist> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.WhiteListExist>(x => aggregateIds.Contains(x.OrderId));
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