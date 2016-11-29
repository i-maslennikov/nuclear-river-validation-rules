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
    public sealed class LegalPersonAccessor : IStorageBasedDataObjectAccessor<LegalPerson>, IDataChangesHandler<LegalPerson>
    {
        private readonly IQuery _query;

        public LegalPersonAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<LegalPerson> GetSource() => _query
            .For<Erm::LegalPerson>()
            .Where(x => x.IsActive && !x.IsDeleted)
            .Select(x => new LegalPerson
                {
                    Id = x.Id
                });

        public FindSpecification<LegalPerson> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<LegalPerson>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<LegalPerson> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<LegalPerson> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<LegalPerson> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<LegalPerson> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id);

            var orderIds =
                from order in _query.For<Order>().Where(x => ids.Contains(x.LegalPersonId.Value))
                select order.Id;

            return new EventCollectionHelper { { typeof(Order), orderIds } };
        }
    }
}