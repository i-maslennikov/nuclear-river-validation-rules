﻿using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Aggregates
{
    public sealed class OrderAggregateRootActor : EntityActorBase<Order>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<Order.InvalidFirm> _orderInvalidFirmRepository;
        private readonly IBulkRepository<Order.InvalidFirmAddress> _orderInvalidFirmAddressRepository;
        private readonly IBulkRepository<Order.BargainSignedLaterThanOrder> _orderBargainSignedLaterThanOrderRepository;
        private readonly IBulkRepository<Order.HasNoAnyLegalPersonProfile> _orderHasNoAnyLegalPersonProfileRepository;
        private readonly IBulkRepository<Order.HasNoAnyPosition> _orderHasNoAnyPositionRepository;
        private readonly IBulkRepository<Order.InvalidBeginDistributionDate> _orderInvalidBeginDistributionDateRepository;
        private readonly IBulkRepository<Order.InvalidEndDistributionDate> _orderInvalidEndDistributionDateRepository;
        private readonly IBulkRepository<Order.InvalidBillsPeriod> _orderInvalidBillsPeriodRepository;
        private readonly IBulkRepository<Order.InvalidBillsTotal> _orderInvalidBillsTotalRepository;
        private readonly IBulkRepository<Order.LegalPersonProfileBargainExpired> _orderLegalPersonProfileBargainEndDateIsEarlierThanOrderSignupDateRepository;
        private readonly IBulkRepository<Order.LegalPersonProfileWarrantyExpired> _orderLegalPersonProfileWarrantyEndDateIsEarlierThanOrderSignupDateRepository;
        private readonly IBulkRepository<Order.MissingBargainScan> _orderMissingBargainScanRepository;
        private readonly IBulkRepository<Order.MissingBills> _orderMissingBillsRepository;
        private readonly IBulkRepository<Order.MissingRequiredField> _orderMissingRequiredFieldRepository;
        private readonly IBulkRepository<Order.MissingOrderScan> _orderMissingOrderScanRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;

        public OrderAggregateRootActor(
            IQuery query,
            IBulkRepository<Order> orderBulkRepository,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Order.InvalidFirm> orderInvalidFirmRepository,
            IBulkRepository<Order.InvalidFirmAddress> orderInvalidFirmAddressRepository,
            IBulkRepository<Order.BargainSignedLaterThanOrder> orderBargainSignedLaterThanOrderRepository,
            IBulkRepository<Order.HasNoAnyLegalPersonProfile> orderHasNoAnyLegalPersonProfileRepository,
            IBulkRepository<Order.HasNoAnyPosition> orderHasNoAnyPositionRepository,
            IBulkRepository<Order.InvalidBeginDistributionDate> orderInvalidBeginDistributionDateRepository,
            IBulkRepository<Order.InvalidEndDistributionDate> orderInvalidEndDistributionDateRepository,
            IBulkRepository<Order.InvalidBillsPeriod> orderInvalidBillsPeriodRepository,
            IBulkRepository<Order.InvalidBillsTotal> orderInvalidBillsTotalRepository,
            IBulkRepository<Order.LegalPersonProfileBargainExpired> orderLegalPersonProfileBargainEndDateIsEarlierThanOrderSignupDateRepository,
            IBulkRepository<Order.LegalPersonProfileWarrantyExpired> orderLegalPersonProfileWarrantyEndDateIsEarlierThanOrderSignupDateRepository,
            IBulkRepository<Order.MissingBargainScan> orderMissingBargainScanRepository,
            IBulkRepository<Order.MissingBills> orderMissingBillsRepository,
            IBulkRepository<Order.MissingRequiredField> orderMissingRequiredFieldRepository,
            IBulkRepository<Order.MissingOrderScan> orderMissingOrderScanRepository)
            : base(query, orderBulkRepository, equalityComparerFactory, new OrderAccessor(query))
        {
            _query = query;
            _equalityComparerFactory = equalityComparerFactory;
            _orderInvalidFirmRepository = orderInvalidFirmRepository;
            _orderInvalidFirmAddressRepository = orderInvalidFirmAddressRepository;
            _orderBargainSignedLaterThanOrderRepository = orderBargainSignedLaterThanOrderRepository;
            _orderHasNoAnyLegalPersonProfileRepository = orderHasNoAnyLegalPersonProfileRepository;
            _orderHasNoAnyPositionRepository = orderHasNoAnyPositionRepository;
            _orderInvalidBeginDistributionDateRepository = orderInvalidBeginDistributionDateRepository;
            _orderInvalidEndDistributionDateRepository = orderInvalidEndDistributionDateRepository;
            _orderInvalidBillsPeriodRepository = orderInvalidBillsPeriodRepository;
            _orderInvalidBillsTotalRepository = orderInvalidBillsTotalRepository;
            _orderLegalPersonProfileBargainEndDateIsEarlierThanOrderSignupDateRepository = orderLegalPersonProfileBargainEndDateIsEarlierThanOrderSignupDateRepository;
            _orderLegalPersonProfileWarrantyEndDateIsEarlierThanOrderSignupDateRepository = orderLegalPersonProfileWarrantyEndDateIsEarlierThanOrderSignupDateRepository;
            _orderMissingBargainScanRepository = orderMissingBargainScanRepository;
            _orderMissingBillsRepository = orderMissingBillsRepository;
            _orderMissingRequiredFieldRepository = orderMissingRequiredFieldRepository;
            _orderMissingOrderScanRepository = orderMissingOrderScanRepository;
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<Order.InvalidFirm>(_query, _orderInvalidFirmRepository, _equalityComparerFactory, new InvalidFirmAccessor (_query)),
                    new ValueObjectActor<Order.InvalidFirmAddress>(_query, _orderInvalidFirmAddressRepository, _equalityComparerFactory, new InvalidFirmAddressAccessor (_query)),
                    new ValueObjectActor<Order.BargainSignedLaterThanOrder>(_query, _orderBargainSignedLaterThanOrderRepository, _equalityComparerFactory, new OrderBargainSignedLaterThanOrderAccessor (_query)),
                    new ValueObjectActor<Order.HasNoAnyLegalPersonProfile>(_query, _orderHasNoAnyLegalPersonProfileRepository, _equalityComparerFactory, new OrderHasNoAnyLegalPersonProfileAccessor (_query)),
                    new ValueObjectActor<Order.HasNoAnyPosition>(_query, _orderHasNoAnyPositionRepository, _equalityComparerFactory, new OrderHasNoAnyPositionAccessor (_query)),
                    new ValueObjectActor<Order.InvalidBeginDistributionDate>(_query, _orderInvalidBeginDistributionDateRepository, _equalityComparerFactory, new OrderInvalidBeginDistributionDateAccessor (_query)),
                    new ValueObjectActor<Order.InvalidEndDistributionDate>(_query, _orderInvalidEndDistributionDateRepository, _equalityComparerFactory, new OrderInvalidEndDistributionDateAccessor (_query)),
                    new ValueObjectActor<Order.InvalidBillsPeriod>(_query, _orderInvalidBillsPeriodRepository, _equalityComparerFactory, new OrderInvalidBillsPeriodAccessor (_query)),
                    new ValueObjectActor<Order.InvalidBillsTotal>(_query, _orderInvalidBillsTotalRepository, _equalityComparerFactory, new OrderInvalidBillsTotalAccessor (_query)),
                    new ValueObjectActor<Order.LegalPersonProfileBargainExpired>(_query, _orderLegalPersonProfileBargainEndDateIsEarlierThanOrderSignupDateRepository, _equalityComparerFactory, new OrderLegalPersonProfileBargainEndDateIsEarlierThanOrderSignupDateAccessor (_query)),
                    new ValueObjectActor<Order.LegalPersonProfileWarrantyExpired>(_query, _orderLegalPersonProfileWarrantyEndDateIsEarlierThanOrderSignupDateRepository, _equalityComparerFactory, new OrderLegalPersonProfileWarrantyEndDateIsEarlierThanOrderSignupDateAccessor (_query)),
                    new ValueObjectActor<Order.MissingBargainScan>(_query, _orderMissingBargainScanRepository, _equalityComparerFactory, new OrderMissingBargainScanAccessor (_query)),
                    new ValueObjectActor<Order.MissingBills>(_query, _orderMissingBillsRepository, _equalityComparerFactory, new OrderMissingBillsAccessor (_query)),
                    new ValueObjectActor<Order.MissingRequiredField>(_query, _orderMissingRequiredFieldRepository, _equalityComparerFactory, new MissingRequiredFieldAccessor(_query)),
                    new ValueObjectActor<Order.MissingOrderScan>(_query, _orderMissingOrderScanRepository, _equalityComparerFactory, new OrderMissingOrderScanAccessor (_query)),
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
                           ProjectId = project.Id,
                           BeginDistribution = order.BeginDistribution,
                           EndDistributionFact = order.EndDistributionFact,
                           EndDistributionPlan = order.EndDistributionPlan,
                           Number = order.Number,
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

        public sealed class InvalidFirmAccessor : IStorageBasedDataObjectAccessor<Order.InvalidFirm>
        {
            private readonly IQuery _query;

            public InvalidFirmAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.InvalidFirm> GetSource()
                => from order in _query.For<Facts::Order>()
                   from firm in _query.For<Facts::Firm>().Where(x => x.Id == order.FirmId)
                   let state = firm.IsDeleted ? InvalidFirmState.Deleted
                                   : !firm.IsActive ? InvalidFirmState.ClosedForever
                                   : firm.IsClosedForAscertainment ? InvalidFirmState.ClosedForAscertainment 
                                   : InvalidFirmState.NotSet
                   where state != InvalidFirmState.NotSet // todo: интересно было бы глянуть на сгенерированный sql
                   select new Order.InvalidFirm
                       {
                           FirmId = firm.Id,
                           FirmName = firm.Name,
                           OrderId = order.Id,
                           State = state,
                       };

            public FindSpecification<Order.InvalidFirm> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.InvalidFirm>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class InvalidFirmAddressAccessor : IStorageBasedDataObjectAccessor<Order.InvalidFirmAddress>
        {
            private readonly IQuery _query;

            public InvalidFirmAddressAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.InvalidFirmAddress> GetSource()
                => from order in _query.For<Facts::Order>()
                   from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                   from opa in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id)
                   from position in _query.For<Facts::Position>().Where(x => x.Id == opa.PositionId)
                   from address in _query.For<Facts::FirmAddress>().Where(x => x.Id == opa.FirmAddressId)
                   let state = address.FirmId != order.FirmId ? InvalidFirmAddressState.NotBelongToFirm
                                : address.IsDeleted ? InvalidFirmAddressState.Deleted
                                : !address.IsActive ? InvalidFirmAddressState.NotActive
                                : address.IsClosedForAscertainment ? InvalidFirmAddressState.ClosedForAscertainment
                                : InvalidFirmAddressState.NotSet
                   where state != InvalidFirmAddressState.NotSet // todo: интересно было бы глянуть на сгенерированный sql
                   select new Order.InvalidFirmAddress
                       {
                           OrderId = order.Id,
                           FirmAddressId = address.Id,
                           FirmAddressName = address.Name,
                           OrderPositionId = orderPosition.Id,
                           OrderPositionName = position.Name,
                           State = state,
                       };

            public FindSpecification<Order.InvalidFirmAddress> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.InvalidFirmAddress>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class InvalidCategoryFirmAddressAccessor : IStorageBasedDataObjectAccessor<Order.InvalidCategoryFirmAddress>
        {
            private readonly IQuery _query;

            public InvalidCategoryFirmAddressAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.InvalidCategoryFirmAddress> GetSource()
                => from order in _query.For<Facts::Order>()
                   from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                   from opa in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id && x.CategoryId.HasValue && x.FirmAddressId.HasValue)
                   from position in _query.For<Facts::Position>().Where(x => x.Id == opa.PositionId)
                   from address in _query.For<Facts::FirmAddress>().Where(x => x.Id == opa.FirmAddressId && x.IsActive && !x.IsClosedForAscertainment && !x.IsDeleted)
                   from category in _query.For<Facts::Category>().Where(x => x.Id == opa.CategoryId)
                   from cfa in _query.For<Facts::CategoryFirmAddress>().Where(x => x.FirmAddressId == opa.FirmAddressId && x.CategoryId == opa.CategoryId).DefaultIfEmpty()
                   where cfa == null
                   select new Order.InvalidCategoryFirmAddress
                   {
                       OrderId = order.Id,
                       FirmAddressId = address.Id,
                       FirmAddressName = address.Name,
                       CategoryId = category.Id,
                       CategoryName = category.Name,
                       OrderPositionId = orderPosition.Id,
                       OrderPositionName = position.Name,
                       State = InvalidCategoryFirmAddressState.CategoryNotBelongsToAddress,
                   };

            public FindSpecification<Order.InvalidCategoryFirmAddress> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.InvalidCategoryFirmAddress>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class InvalidCategoryAccessor : IStorageBasedDataObjectAccessor<Order.InvalidCategory>
        {
            private const int BindingObjectTypeCategoryMultipleAsterix = 1;

            private readonly IQuery _query;

            public InvalidCategoryAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.InvalidCategory> GetSource()
                => from order in _query.For<Facts::Order>()
                   from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                   from opa in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id && x.CategoryId.HasValue && x.FirmAddressId.HasValue)
                   from category in _query.For<Facts::Category>().Where(x => x.Id == opa.CategoryId)
                   from position in _query.For<Facts::Position>().Where(x => x.Id == opa.PositionId)
                   let categoryBelongToFirmAddress = _query.For<Facts::FirmAddress>()
                                                           .SelectMany(fa => _query.For<Facts::CategoryFirmAddress>().Where(cfa => cfa.FirmAddressId == fa.Id))
                                                           .Any(x => x.CategoryId == opa.CategoryId)
                   let state = !category.IsActiveNotDeleted ? InvalidCategoryState.Inactive
                                   : !categoryBelongToFirmAddress ? InvalidCategoryState.NotBelongToFirm
                                   : InvalidCategoryState.NotSet
                   where state != InvalidCategoryState.NotSet
                   select new Order.InvalidCategory
                       {
                           OrderId = order.Id,
                           CategoryId = category.Id,
                           CategoryName = category.Name,
                           OrderPositionId = orderPosition.Id,
                           OrderPositionName = position.Name,
                           MayNotBelongToFirm = position.BindingObjectType == BindingObjectTypeCategoryMultipleAsterix,
                           State = state,
                       };

            public FindSpecification<Order.InvalidCategory> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.InvalidCategory>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderBargainSignedLaterThanOrderAccessor : IStorageBasedDataObjectAccessor<Order.BargainSignedLaterThanOrder>
        {
            private readonly IQuery _query;

            public OrderBargainSignedLaterThanOrderAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.BargainSignedLaterThanOrder> GetSource()
                => from order in _query.For<Facts::Order>()
                   from bargain in _query.For<Facts::Bargain>().Where(x => x.Id == order.BargainId)
                   where bargain.SignupDate > order.SignupDate
                   select new Order.BargainSignedLaterThanOrder
                   {
                       OrderId = order.Id,
                   };

            public FindSpecification<Order.BargainSignedLaterThanOrder> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.BargainSignedLaterThanOrder>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderHasNoAnyLegalPersonProfileAccessor : IStorageBasedDataObjectAccessor<Order.HasNoAnyLegalPersonProfile>
        {
            private readonly IQuery _query;

            public OrderHasNoAnyLegalPersonProfileAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.HasNoAnyLegalPersonProfile> GetSource()
                => from order in _query.For<Facts::Order>()
                   from profile in _query.For<Facts::LegalPersonProfile>().Where(x => x.LegalPersonId == order.LegalPersonId).DefaultIfEmpty()
                   where profile == null
                   select new Order.HasNoAnyLegalPersonProfile
                   {
                       OrderId = order.Id,
                   };

            public FindSpecification<Order.HasNoAnyLegalPersonProfile> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.HasNoAnyLegalPersonProfile>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderHasNoAnyPositionAccessor : IStorageBasedDataObjectAccessor<Order.HasNoAnyPosition>
        {
            private readonly IQuery _query;

            public OrderHasNoAnyPositionAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.HasNoAnyPosition> GetSource()
                => from order in _query.For<Facts::Order>()
                   from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id).DefaultIfEmpty()
                   where orderPosition == null
                   select new Order.HasNoAnyPosition
                   {
                       OrderId = order.Id,
                   };

            public FindSpecification<Order.HasNoAnyPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.HasNoAnyPosition>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderInvalidBeginDistributionDateAccessor : IStorageBasedDataObjectAccessor<Order.InvalidBeginDistributionDate>
        {
            private readonly IQuery _query;

            public OrderInvalidBeginDistributionDateAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.InvalidBeginDistributionDate> GetSource()
                => from order in _query.For<Facts::Order>()
                   where order.BeginDistribution.Day != 1
                   select new Order.InvalidBeginDistributionDate
                   {
                       OrderId = order.Id,
                   };

            public FindSpecification<Order.InvalidBeginDistributionDate> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.InvalidBeginDistributionDate>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderInvalidEndDistributionDateAccessor : IStorageBasedDataObjectAccessor<Order.InvalidEndDistributionDate>
        {
            private readonly IQuery _query;

            public OrderInvalidEndDistributionDateAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.InvalidEndDistributionDate> GetSource()
                => from order in _query.For<Facts::Order>()
                   where order.EndDistributionPlan != order.BeginDistribution.AddMonths(order.ReleaseCountPlan).AddSeconds(-1)
                   select new Order.InvalidEndDistributionDate
                   {
                       OrderId = order.Id,
                   };

            public FindSpecification<Order.InvalidEndDistributionDate> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.InvalidEndDistributionDate>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderInvalidBillsPeriodAccessor : IStorageBasedDataObjectAccessor<Order.InvalidBillsPeriod>
        {
            private readonly IQuery _query;

            public OrderInvalidBillsPeriodAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.InvalidBillsPeriod> GetSource()
                => from order in _query.For<Facts::Order>()
                   let minimumDate = _query.For<Facts::Bill>().Where(x => x.OrderId == order.Id).Min(x => x.Begin)
                   let maximumDate = _query.For<Facts::Bill>().Where(x => x.OrderId == order.Id).Max(x => x.End)
                   where order.BeginDistribution != minimumDate || order.EndDistributionPlan != maximumDate
                   select new Order.InvalidBillsPeriod
                       {
                           OrderId = order.Id,
                       };

            public FindSpecification<Order.InvalidBillsPeriod> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.InvalidBillsPeriod>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderInvalidBillsTotalAccessor : IStorageBasedDataObjectAccessor<Order.InvalidBillsTotal>
        {
            private const int WorkflowStepOnRegistration = 1;

            private readonly IQuery _query;

            public OrderInvalidBillsTotalAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.InvalidBillsTotal> GetSource()
                => from order in _query.For<Facts::Order>().Where(x => x.WorkflowStep == WorkflowStepOnRegistration)
                   let billTotal = _query.For<Facts::Bill>().Where(x => x.OrderId == order.Id).Sum(x => x.PayablePlan)
                   let orderTotal = (from op in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                                     from rw in _query.For<Facts::ReleaseWithdrawal>().Where(x => x.OrderPositionId == op.Id)
                                     select rw.Amount).Sum()
                   where billTotal != orderTotal
                   select new Order.InvalidBillsTotal
                   {
                       OrderId = order.Id,
                   };

            public FindSpecification<Order.InvalidBillsTotal> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.InvalidBillsTotal>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderLegalPersonProfileBargainEndDateIsEarlierThanOrderSignupDateAccessor : IStorageBasedDataObjectAccessor<Order.LegalPersonProfileBargainExpired>
        {
            private readonly IQuery _query;

            public OrderLegalPersonProfileBargainEndDateIsEarlierThanOrderSignupDateAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.LegalPersonProfileBargainExpired> GetSource()
                => from order in _query.For<Facts::Order>()
                   from profile in _query.For<Facts::LegalPersonProfile>().Where(x => x.LegalPersonId == order.LegalPersonId)
                   where profile.BargainEndDate.HasValue && profile.BargainEndDate.Value < order.SignupDate
                   select new Order.LegalPersonProfileBargainExpired
                   {
                       OrderId = order.Id,
                       LegalPersonProfileId = profile.Id,
                       LegalPersonProfileName = profile.Name,
                   };

            public FindSpecification<Order.LegalPersonProfileBargainExpired> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.LegalPersonProfileBargainExpired>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderLegalPersonProfileWarrantyEndDateIsEarlierThanOrderSignupDateAccessor : IStorageBasedDataObjectAccessor<Order.LegalPersonProfileWarrantyExpired>
        {
            private readonly IQuery _query;

            public OrderLegalPersonProfileWarrantyEndDateIsEarlierThanOrderSignupDateAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.LegalPersonProfileWarrantyExpired> GetSource()
                => from order in _query.For<Facts::Order>()
                   from profile in _query.For<Facts::LegalPersonProfile>().Where(x => x.LegalPersonId == order.LegalPersonId)
                   where profile.WarrantyEndDate.HasValue && profile.WarrantyEndDate.Value < order.SignupDate
                   select new Order.LegalPersonProfileWarrantyExpired
                   {
                       OrderId = order.Id,
                       LegalPersonProfileId = profile.Id,
                       LegalPersonProfileName = profile.Name,
                   };

            public FindSpecification<Order.LegalPersonProfileWarrantyExpired> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.LegalPersonProfileWarrantyExpired>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderMissingBargainScanAccessor : IStorageBasedDataObjectAccessor<Order.MissingBargainScan>
        {
            private readonly IQuery _query;

            public OrderMissingBargainScanAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.MissingBargainScan> GetSource()
                => from order in _query.For<Facts::Order>()
                   from scan in _query.For<Facts::BargainScanFile>().Where(x => x.BargainId == order.BargainId).DefaultIfEmpty()
                   where order.BargainId.HasValue && scan == null
                   select new Order.MissingBargainScan
                   {
                       OrderId = order.Id,
                   };

            public FindSpecification<Order.MissingBargainScan> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.MissingBargainScan>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderMissingBillsAccessor : IStorageBasedDataObjectAccessor<Order.MissingBills>
        {
            private const int WorkflowStepOnRegistration = 1;

            private readonly IQuery _query;

            public OrderMissingBillsAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.MissingBills> GetSource()
                => from order in _query.For<Facts::Order>().Where(x => x.WorkflowStep == WorkflowStepOnRegistration)
                   let billCount = _query.For<Facts::Bill>().Count(x => x.OrderId == order.Id)
                   let orderTotal = (from op in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                                     from rw in _query.For<Facts::ReleaseWithdrawal>().Where(x => x.OrderPositionId == op.Id)
                                     select rw.Amount).Sum()
                   where orderTotal > 0 && !order.IsFreeOfCharge && billCount == 0
                   select new Order.MissingBills
                   {
                       OrderId = order.Id,
                   };

            public FindSpecification<Order.MissingBills> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.MissingBills>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class MissingRequiredFieldAccessor : IStorageBasedDataObjectAccessor<Order.MissingRequiredField>
        {
            private readonly IQuery _query;

            public MissingRequiredFieldAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.MissingRequiredField> GetSource()
                => from order in _query.For<Facts::Order>()
                   where !(order.BranchOfficeOrganizationUnitId.HasValue && order.CurrencyId.HasValue && order.InspectorId.HasValue && order.LegalPersonId.HasValue && order.LegalPersonProfileId.HasValue)
                   select new Order.MissingRequiredField
                       {
                           OrderId = order.Id,
                           BranchOfficeOrganizationUnit = !order.BranchOfficeOrganizationUnitId.HasValue,
                           Currency = !order.CurrencyId.HasValue,
                           Inspector = !order.InspectorId.HasValue,
                           LegalPerson = !order.LegalPersonId.HasValue,
                           LegalPersonProfile = !order.LegalPersonProfileId.HasValue,
                           ReleaseCountPlan = order.ReleaseCountPlan == 0,
                       };

            public FindSpecification<Order.MissingRequiredField> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.MissingRequiredField>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class OrderMissingOrderScanAccessor : IStorageBasedDataObjectAccessor<Order.MissingOrderScan>
        {
            private readonly IQuery _query;

            public OrderMissingOrderScanAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.MissingOrderScan> GetSource()
                => from order in _query.For<Facts::Order>()
                   from scan in _query.For<Facts::OrderScanFile>().Where(x => x.OrderId == order.Id).DefaultIfEmpty()
                   where scan == null
                   select new Order.MissingOrderScan
                   {
                       OrderId = order.Id,
                   };

            public FindSpecification<Order.MissingOrderScan> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.MissingOrderScan>(x => aggregateIds.Contains(x.OrderId));
            }
        }
    }
}