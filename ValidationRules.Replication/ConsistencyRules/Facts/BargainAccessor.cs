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
    public sealed class BargainAccessor : IStorageBasedDataObjectAccessor<Bargain>, IDataChangesHandler<Bargain>
    {
        private readonly IQuery _query;

        public BargainAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Bargain> GetSource()
            => _query.For<Erm::Bargain>()
                     .Where(x => x.IsActive && !x.IsDeleted)
                     .Select(x => new Bargain
                         {
                             Id = x.Id,
                             SignupDate = x.SignedOn,
                         });

        public FindSpecification<Bargain> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Bargain>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Bargain> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Bargain), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Bargain> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Bargain), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Bargain> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Bargain), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Bargain> dataObjects)
            => Array.Empty<IEvent>();
    }
}