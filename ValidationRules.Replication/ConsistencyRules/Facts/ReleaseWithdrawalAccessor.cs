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
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(ReleaseWithdrawal), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<ReleaseWithdrawal> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(ReleaseWithdrawal), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<ReleaseWithdrawal> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(ReleaseWithdrawal), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<ReleaseWithdrawal> dataObjects)
            => Array.Empty<IEvent>();
    }
}