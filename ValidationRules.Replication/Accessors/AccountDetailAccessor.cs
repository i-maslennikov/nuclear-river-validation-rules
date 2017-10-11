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

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class AccountDetailAccessor : IStorageBasedDataObjectAccessor<AccountDetail>, IDataChangesHandler<AccountDetail>
    {
        private readonly IQuery _query;

        public AccountDetailAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<AccountDetail> GetSource()
            => _query.For<Storage.Model.Erm.AccountDetail>()
                     .Where(x => !x.IsDeleted)
                     .Where(x => x.OrderId != null)
                     .Select(x => new AccountDetail
                         {
                             Id = x.Id,
                             AccountId = x.AccountId,
                             OrderId = x.OrderId.Value,
                             PeriodStartDate = x.PeriodStartDate.Value,
                         });

        public FindSpecification<AccountDetail> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToList();
            return SpecificationFactory<AccountDetail>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<AccountDetail> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<AccountDetail> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<AccountDetail> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<AccountDetail> dataObjects)
        {
            var accountIds = dataObjects.Select(x => x.AccountId);
            return new EventCollectionHelper<AccountDetail> { { typeof(Account), accountIds } };
        }
    }
}