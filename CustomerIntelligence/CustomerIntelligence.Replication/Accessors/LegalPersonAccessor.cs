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
    public sealed class LegalPersonAccessor : IStorageBasedDataObjectAccessor<LegalPerson>, IDataChangesHandler<LegalPerson>
    {
        private readonly IQuery _query;

        public LegalPersonAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<LegalPerson> GetSource() => Specs.Map.Erm.ToFacts.LegalPersons.Map(_query);

        public FindSpecification<LegalPerson> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            => new FindSpecification<LegalPerson>(x => commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).Contains(x.Id));

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<LegalPerson> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<LegalPerson> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<LegalPerson> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<LegalPerson> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<LegalPerson>(x => ids.Contains(x.Id));

            return Specs.Map.Facts.ToFirmAggregate.ByLegalPerson(specification)
                        .Map(_query)
                        .Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Firm), x))
                        .ToArray();
        }
    }
}