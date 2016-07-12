using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.User.Facts;

namespace NuClear.ValidationRules.Replication.User
{
    public sealed class OrderPositionAccessor : IStorageBasedDataObjectAccessor<AccountOrder>, IDataChangesHandler<AccountOrder>
    {
        private readonly IQuery _query;

        public OrderPositionAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<AccountOrder> GetSource()
            => _query.For(Specs.Find.Erm.Orders())
                     .Select(x => new AccountOrder { OrderId = x.Id, UserAccountId = x.OwnerCode });

        public FindSpecification<AccountOrder> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<AccountOrder>(x => ids.Contains(x.OrderId));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<AccountOrder> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<AccountOrder> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<AccountOrder> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<AccountOrder> dataObjects)
            => Array.Empty<IEvent>();
    }
}