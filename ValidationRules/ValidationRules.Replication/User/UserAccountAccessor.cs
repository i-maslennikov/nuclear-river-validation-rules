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
    public sealed class UserAccountAccessor : IStorageBasedDataObjectAccessor<UserAccount>, IDataChangesHandler<UserAccount>
    {
        private readonly IQuery _query;

        public UserAccountAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<UserAccount> GetSource()
            => _query.For(Specs.Find.Erm.Users())
                     .Select(x => new UserAccount { Id = x.Id, Name = x.Account });

        public FindSpecification<UserAccount> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<UserAccount>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<UserAccount> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<UserAccount> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<UserAccount> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<UserAccount> dataObjects)
            => Array.Empty<IEvent>();
    }
}