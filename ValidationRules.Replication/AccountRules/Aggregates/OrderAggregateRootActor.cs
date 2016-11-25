using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Facts = NuClear.ValidationRules.Storage.Model.AccountRules.Facts;

namespace NuClear.ValidationRules.Replication.AccountRules.Aggregates
{
    public sealed class OrderAggregateRootActor : AggregateRootActor<Order>
    {
        public OrderAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Order> orderBulkRepository,
            IBulkRepository<Lock> lockBulkRepository,
            IBulkRepository<Order.DebtPermission> debtPermissionBulkRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new OrderAccessor(query), orderBulkRepository,
                HasValueObject(new LockAccessor(query), lockBulkRepository),
                HasValueObject(new DebtPermissionAccessor(query), debtPermissionBulkRepository));
        }

        public sealed class OrderAccessor : DataChangesHandler<Order>, IStorageBasedDataObjectAccessor<Order>
        {
            private readonly IQuery _query;

            public OrderAccessor(IQuery query)
            {
                Invalidate(MessageTypeCode.AccountBalanceShouldBePositive);
                Invalidate(MessageTypeCode.AccountShouldExist);
                Invalidate(MessageTypeCode.LockShouldNotExist);

                _query = query;
            }

            public IQueryable<Order> GetSource()
                => from order in _query.For<Facts::Order>()
                   join destProject in _query.For<Facts::Project>() on order.DestOrganizationUnitId equals destProject.OrganizationUnitId
                   join sourceProject in _query.For<Facts::Project>() on order.SourceOrganizationUnitId equals sourceProject.OrganizationUnitId
                   from account in _query.For<Facts::Account>().Where(x => x.LegalPersonId == order.LegalPersonId && x.BranchOfficeOrganizationUnitId == order.BranchOfficeOrganizationUnitId).DefaultIfEmpty()
                   select new Order
                       {
                           Id = order.Id,
                           DestProjectId = destProject.Id,
                           SourceProjectId = sourceProject.Id,
                           AccountId = account.Id,
                           IsFreeOfCharge = order.IsFreeOfCharge,
                           Number = order.Number,
                           BeginDistributionDate = order.BeginDistributionDate,
                           EndDistributionDate = order.EndDistributionDate,
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

        public sealed class LockAccessor : DataChangesHandler<Lock>, IStorageBasedDataObjectAccessor<Lock>
        {
            private readonly IQuery _query;

            public LockAccessor(IQuery query)
            {
                Invalidate(MessageTypeCode.LockShouldNotExist);

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
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Lock>(x => aggregateIds.Contains(x.OrderId));
            }
        }

        public sealed class DebtPermissionAccessor : DataChangesHandler<Order.DebtPermission>, IStorageBasedDataObjectAccessor<Order.DebtPermission>
        {
            private readonly IQuery _query;

            public DebtPermissionAccessor(IQuery query)
            {
                Invalidate(MessageTypeCode.AccountBalanceShouldBePositive);

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
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Order.DebtPermission>(x => aggregateIds.Contains(x.OrderId));
            }
        }
    }
}