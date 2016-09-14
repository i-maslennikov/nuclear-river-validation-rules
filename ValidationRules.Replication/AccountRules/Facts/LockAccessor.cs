﻿using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.AccountRules.Facts;

namespace NuClear.ValidationRules.Replication.AccountRules.Facts
{
    public sealed class LockAccessor : IStorageBasedDataObjectAccessor<Lock>, IDataChangesHandler<Lock>
    {
        private readonly IQuery _query;

        public LockAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Lock> GetSource()
            => _query.For(Specs.Find.Erm.Locks()).Select(x => new Lock
                {
                    Id = x.Id,
                    OrderId = x.OrderId,
                    AccountId = x.AccountId,
                    Amount = x.PlannedAmount,
                    Start = x.PeriodStartDate,
                });

        public FindSpecification<Lock> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Lock>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Lock> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Lock> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Lock> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Lock> dataObjects)
            // полагаться на поля, отличные от Id не стоит, но здесь расчёт на то, что блокировку нельзя перевести с одного ЛС или заказа на другой
            => dataObjects.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Order), x.OrderId))
                          .Union(dataObjects.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Account), x.AccountId)))
                          .ToArray();
    }
}