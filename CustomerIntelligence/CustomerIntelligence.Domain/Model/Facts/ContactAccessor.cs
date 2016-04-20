using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Events;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class ContactAccessor : IStorageBasedDataObjectAccessor<Contact>, IDataChangesHandler<Contact>
    {
        private readonly IQuery _query;

        public ContactAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Contact> GetSource() => Specs.Map.Erm.ToFacts.Contacts.Map(_query);

        public FindSpecification<Contact> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            => new FindSpecification<Contact>(x => commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).Contains(x.Id));

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Contact> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Contact), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Contact> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Contact), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Contact> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Contact), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Contact> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<Contact>(x => ids.Contains(x.Id));

            IEnumerable<IEvent> events = Specs.Map.Facts.ToClientAggregate.ByContacts(specification)
                                              .Map(_query)
                                              .Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Client), x));

            events = events.Concat(Specs.Map.Facts.ToFirmAggregate.ByContacts(specification)
                                        .Map(_query)
                                        .Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Firm), x)));
            return events.ToArray();
        }
    }
}