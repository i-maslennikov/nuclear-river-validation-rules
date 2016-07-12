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
    public sealed class OrderPositionAccessor : IStorageBasedDataObjectAccessor<UserOrder>, IDataChangesHandler<UserOrder>
    {
        private readonly IQuery _query;

        public OrderPositionAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<UserOrder> GetSource()
            => _query.For(Specs.Find.Erm.Orders())
                     .Select(x => new UserOrder { OrderId = x.Id, UserId = x.OwnerCode });

        public FindSpecification<UserOrder> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<UserOrder>(x => ids.Contains(x.OrderId));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<UserOrder> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<UserOrder> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<UserOrder> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<UserOrder> dataObjects)
            => Array.Empty<IEvent>();
    }
}