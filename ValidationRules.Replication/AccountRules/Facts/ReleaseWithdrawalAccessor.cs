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
    public sealed class ReleaseWithdrawalAccessor : IStorageBasedDataObjectAccessor<ReleaseWithdrawal>, IDataChangesHandler<ReleaseWithdrawal>
    {
        private readonly IQuery _query;

        public ReleaseWithdrawalAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<ReleaseWithdrawal> GetSource()

            => from releaseWithdrawal in _query.For(Specs.Find.Erm.ReleaseWithdrawals())
               join orderPosition in _query.For(Specs.Find.Erm.OrderPositions()) on releaseWithdrawal.OrderPositionId equals orderPosition.Id
               join order in _query.For(Specs.Find.Erm.Orders()) on orderPosition.OrderId equals order.Id // Полумера, сокращает начальный объём фактов, но не защищает от его роста
               select new ReleaseWithdrawal
                   {
                       Id = releaseWithdrawal.Id,
                       OrderPositionId = releaseWithdrawal.OrderPositionId,
                       Amount = releaseWithdrawal.AmountToWithdraw,
                       Start = releaseWithdrawal.ReleaseBeginDate,
                   };

        public FindSpecification<ReleaseWithdrawal> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<ReleaseWithdrawal>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<ReleaseWithdrawal> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<ReleaseWithdrawal> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<ReleaseWithdrawal> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<ReleaseWithdrawal> dataObjects)
        {
            var orderPositionIds = dataObjects.Select(x => x.OrderPositionId);

            var accountIds =
                from order in _query.For<Order>()
                from account in _query.For<Account>().Where(x => x.LegalPersonId == order.LegalPersonId && x.BranchOfficeOrganizationUnitId == order.BranchOfficeOrganizationUnitId)
                join orderPosition in _query.For<OrderPosition>().Where(x => orderPositionIds.Contains(x.Id)) on order.Id equals orderPosition.OrderId
                select account.Id;

            accountIds = accountIds.Distinct();

            return accountIds.Select(id => new RelatedDataObjectOutdatedEvent<long>(typeof(Account), id)).ToArray();
        }
    }
}