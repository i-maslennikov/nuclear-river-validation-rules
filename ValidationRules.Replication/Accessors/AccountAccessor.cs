using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class AccountAccessor : IStorageBasedDataObjectAccessor<Account>, IDataChangesHandler<Account>
    {
        private readonly IQuery _query;

        public AccountAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Account> GetSource()
            => _query.For<Erm::Account>()
                .Where(x => !x.IsArchived)
                .Select(x => new Account
                {
                    Id = x.Id,
                    Balance = x.Balance,
                    BranchOfficeOrganizationUnitId = x.BranchOfficeOrganizationUnitId,
                    LegalPersonId = x.LegalPersonId
                });

        public FindSpecification<Account> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Account>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Account> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Account), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Account> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Account), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Account> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Account), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Account> dataObjects)
        {
            var accountIds = dataObjects.Select(x => x.Id).ToArray();

            var orderIds =
                from account in _query.For<Account>().Where(x => accountIds.Contains(x.Id))
                from order in _query.For<Order>().Where(x => x.LegalPersonId == account.LegalPersonId && x.BranchOfficeOrganizationUnitId == account.BranchOfficeOrganizationUnitId)
                select order.Id;

            return new EventCollectionHelper { { typeof(Order), orderIds.Distinct() } };
        }
    }
}