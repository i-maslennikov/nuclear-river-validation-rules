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
using NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.AccountRules.Aggregates
{
    public sealed class OrderAggregateRootActor : EntityActorBase<Order>, IAggregateRootActor
    {
        private const int OrderOnTermination = 4;
        private const int OrderApproved = 5;

        private readonly IQuery _query;
        private readonly IEqualityComparerFactory _equalityComparerFactory;
        private readonly IBulkRepository<Lock> _lockBulkRepository;
        private readonly IBulkRepository<Order.DebtPermission> _debtPermissionBulkRepository;

        public OrderAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Order> orderBulkRepository,
            IBulkRepository<Lock> lockBulkRepository,
            IBulkRepository<Order.DebtPermission> debtPermissionBulkRepository)
            : base(query, orderBulkRepository, equalityComparerFactory, new OrderAccessor(query))
        {
            _query = query;
            _equalityComparerFactory = equalityComparerFactory;
            _lockBulkRepository = lockBulkRepository;
            _debtPermissionBulkRepository = debtPermissionBulkRepository;
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<Lock>(_query, _lockBulkRepository, _equalityComparerFactory, new LockAccessor(_query)),
                    new ValueObjectActor<Order.DebtPermission>(_query, _debtPermissionBulkRepository, _equalityComparerFactory, new DebtPermissionAccessor(_query)),
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
                   where order.WorkflowStep == OrderOnTermination || order.WorkflowStep == OrderApproved
                   join destProject in _query.For<Facts::Project>() on order.DestOrganizationUnitId equals destProject.OrganizationUnitId
                   from account in _query.For<Facts::Account>().Where(x => x.LegalPersonId == order.LegalPersonId && x.BranchOfficeOrganizationUnitId == order.BranchOfficeOrganizationUnitId).DefaultIfEmpty()
                   select new Order
                       {
                           Id = order.Id,
                           DestProjectId = destProject.Id,
                           AccountId = account.Id,
                           IsFreeOfCharge = order.IsFreeOfCharge,
                           Number = order.Number,
                           BeginDistributionDate = order.BeginDistribution,
                           EndDistributionDate = order.EndDistributionFact,
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

        public sealed class LockAccessor : IStorageBasedDataObjectAccessor<Lock>
        {
            private readonly IQuery _query;

            public LockAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Lock> GetSource()
                => _query.For<Facts::Lock>().Select(x => new Lock
                    {
                        OrderId = x.OrderId,
                        Start = x.Start,
                        End = x.Start.AddMonths(1)
                });

            public FindSpecification<Lock> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Lock>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class DebtPermissionAccessor : IStorageBasedDataObjectAccessor<Order.DebtPermission>
        {
            private readonly IQuery _query;

            public DebtPermissionAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Order.DebtPermission> GetSource()
                => from x in _query.For<Facts::UnlimitedOrder>()
                   select new Order.DebtPermission
                       {
                           OrderId = x.OrderId,
                           Start = x.PeriodStart,
                           End = x.PeriodEnd,
                       };

            public FindSpecification<Order.DebtPermission> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Order.DebtPermission>(x => aggregateIds.Contains(x.OrderId));
            }
        }
    }
}