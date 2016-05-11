using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.CustomerIntelligence.Replication.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Replication.Accessors
{
    public class FirmContactAccessor : IStorageBasedDataObjectAccessor<FirmContact>, IDataChangesHandler<FirmContact>
    {
        private readonly IQuery _query;

        public FirmContactAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<FirmContact> GetSource() => Specs.Map.Erm.ToFacts.FirmContacts.Map(_query);

        public FindSpecification<FirmContact> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<FirmContact>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<FirmContact> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmContact> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmContact> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<FirmContact> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<FirmContact>(x => ids.Contains(x.Id));

            var firmIds = (from firmAddress in _query.For<FirmAddress>()
                           join firmContact in _query.For(specification) on firmAddress.Id equals firmContact.FirmAddressId
                           select firmAddress.FirmId)
                .ToArray();

            return firmIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Firm), x))
                          .ToArray();
        }
    }
}