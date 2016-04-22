using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Events;
using NuClear.CustomerIntelligence.Domain.Model.Facts;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API.DataObjects;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Accessors
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