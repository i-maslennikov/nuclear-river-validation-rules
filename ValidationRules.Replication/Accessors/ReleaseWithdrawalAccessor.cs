using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class ReleaseWithdrawalAccessor : IStorageBasedDataObjectAccessor<ReleaseWithdrawal>, IDataChangesHandler<ReleaseWithdrawal>
    {
        private readonly IQuery _query;

        public ReleaseWithdrawalAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<ReleaseWithdrawal> GetSource() => _query
            .For<Erm::ReleaseWithdrawal>()
            .Select(x => new ReleaseWithdrawal
            {
                Id = x.Id,
                OrderPositionId = x.OrderPositionId,
                Amount = x.AmountToWithdraw,
                Start = x.ReleaseBeginDate,
            });

        public FindSpecification<ReleaseWithdrawal> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return Specification<ReleaseWithdrawal>.Create(x => x.Id, ids);
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
                from orderPosition in _query.For<OrderPosition>().Where(x => orderPositionIds.Contains(x.Id) && x.OrderId == order.Id)
                select account.Id;

            var orderIds =
                from orderPosition in _query.For<OrderPosition>().Where(x => orderPositionIds.Contains(x.Id))
                select orderPosition.OrderId;

            return new EventCollectionHelper { { typeof(Account), accountIds.Distinct() }, { typeof(Order), orderIds.Distinct() } };
        }
    }
}