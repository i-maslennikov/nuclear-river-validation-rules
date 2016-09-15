using System;
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
    public sealed class LimitAccessor : IStorageBasedDataObjectAccessor<Limit>, IDataChangesHandler<Limit>
    {
        private readonly IQuery _query;

        public LimitAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Limit> GetSource()
            => _query.For(Specs.Find.Erm.Limits()).Select(x => new Limit
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    Amount = x.Amount,
                    Start = x.StartPeriodDate,
                });

        public FindSpecification<Limit> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Limit>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Limit> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Limit> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Limit> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Limit> dataObjects)
        {
            // полагаться на поля, отличные от Id не стоит, но здесь расчёт на то, что лимит нельзя перевести с одного ЛС на другой
            var accountIds = dataObjects.Select(x => x.AccountId);

            return new EventCollectionHelper { { typeof(Account), accountIds.Distinct() } }.ToArray();
        }
    }
}