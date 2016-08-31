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
    public sealed class LegalPersonProfileAccessor : IStorageBasedDataObjectAccessor<LegalPersonProfile>, IDataChangesHandler<LegalPersonProfile>
    {
        private readonly IQuery _query;

        public LegalPersonProfileAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<LegalPersonProfile> GetSource()
            => _query.For<Erm::LegalPersonProfile>()
                     .Where(x => x.IsActive && !x.IsDeleted)
                     .Select(x => new LegalPersonProfile
                         {
                             Id = x.Id,
                             LegalPersonId = x.LegalPersonId,
                             BargainEndDate = x.BargainEndDate,
                             WarrantyEndDate = x.WarrantyEndDate,
                         });

        public FindSpecification<LegalPersonProfile> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<LegalPersonProfile>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<LegalPersonProfile> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(LegalPersonProfile), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<LegalPersonProfile> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(LegalPersonProfile), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<LegalPersonProfile> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(LegalPersonProfile), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<LegalPersonProfile> dataObjects)
            => Array.Empty<IEvent>();
    }
}