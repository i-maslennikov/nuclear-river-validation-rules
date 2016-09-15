using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Facts
{
    public sealed class ReleaseWithdrawalAccessor : IStorageBasedDataObjectAccessor<ReleaseWithdrawal>, IDataChangesHandler<ReleaseWithdrawal>
    {
        private readonly IQuery _query;

        public ReleaseWithdrawalAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<ReleaseWithdrawal> GetSource()
            => _query.For<Erm::ReleaseWithdrawal>()
                     .Select(x => new ReleaseWithdrawal
                         {
                             Id = x.Id,
                             OrderPositionId = x.OrderPositionId,
                             Amount = x.AmountToWithdraw,
                         });

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
            var orderPositionIds = dataObjects.Select(x => x.OrderPositionId).ToArray();

            var orderIds =
                from orderPosition in _query.For<OrderPosition>().Where(x => orderPositionIds.Contains(x.Id))
                select orderPosition.OrderId;

            return new EventCollectionHelper { { typeof(Order), orderIds.Distinct() } };
        }
    }
}