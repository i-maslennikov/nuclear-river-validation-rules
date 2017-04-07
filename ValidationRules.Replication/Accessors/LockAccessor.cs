using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Specs;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class LockAccessor : IStorageBasedDataObjectAccessor<Lock>, IDataChangesHandler<Lock>
    {
        private readonly IQuery _query;

        public LockAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Lock> GetSource() =>
            from @lock in _query.For<Erm::Lock>().Where(x => x.IsActive && !x.IsDeleted)
            from order in _query.For<Erm::Order>().Where(x => x.Id == @lock.OrderId) // тип заказа после создания блокировки не меняется, поэтому join допустим
            select new Lock
                {
                    Id = @lock.Id,
                    OrderId = @lock.OrderId,
                    AccountId = @lock.AccountId,
                    Amount = @lock.PlannedAmount,
                    Start = @lock.PeriodStartDate,
                    IsOrderFreeOfCharge = Erm::Order.FreeOfChargeTypes.Contains(order.OrderType)
                };

        public FindSpecification<Lock> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToList();
            return SpecificationFactory<Lock>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Lock> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Lock> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Lock> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Lock> dataObjects)
        {
            // полагаться на поля, отличные от Id не стоит, но здесь расчёт на то, что блокировку нельзя перевести с одного ЛС или заказа на другой
            var orderIds = dataObjects.Select(x => x.OrderId);
            var accountIds = dataObjects.Select(x => x.AccountId);

            return new EventCollectionHelper<Lock> { { typeof(Order), orderIds }, { typeof(Account), accountIds } };
        }
    }
}