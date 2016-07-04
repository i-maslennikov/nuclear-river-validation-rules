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

using Facts = NuClear.ValidationRules.Storage.Model.AccountRules.Facts;

namespace NuClear.ValidationRules.Replication.AccountRules.Aggregates
{
    public sealed class OrderAggregateRootActor : EntityActorBase<Order>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IEqualityComparerFactory _equalityComparerFactory;
        private readonly IBulkRepository<Lock> _lockBulkRepository;

        public OrderAggregateRootActor(
            IQuery query,
            IBulkRepository<Order> orderBulkRepository,
            IBulkRepository<Lock> lockBulkRepository,
            IEqualityComparerFactory equalityComparerFactory)
            : base(query, orderBulkRepository, equalityComparerFactory, new OrderAccessor(query))
        {
            _query = query;
            _equalityComparerFactory = equalityComparerFactory;
            _lockBulkRepository = lockBulkRepository;
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<Lock>(_query, _lockBulkRepository, _equalityComparerFactory, new LockAccessor(_query))
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
                   join project in _query.For<Facts::Project>() on order.DestOrganizationUnitId equals project.OrganizationUnitId
                   from account in _query.For<Facts::Account>().Where(x => x.Id == order.AccountId).DefaultIfEmpty()
                   select new Order
                       {
                           Id = order.Id,
                           ProjectId = project.Id,
                           Number = order.Number,
                           BeginDistributionDate = order.BeginDistributionDate,
                           EndDistributionDatePlan = order.EndDistributionDatePlan,
                           HasAccount = account != null
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
                        Start = x.PeriodStartDate,
                        End = x.PeriodEndDate,
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
    }
}