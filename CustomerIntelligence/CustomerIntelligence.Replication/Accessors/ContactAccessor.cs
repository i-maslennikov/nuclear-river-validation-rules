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
    public sealed class ContactAccessor : IStorageBasedDataObjectAccessor<Contact>, IDataChangesHandler<Contact>
    {
        private readonly IQuery _query;

        public ContactAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Contact> GetSource() => Specs.Map.Erm.ToFacts.Contacts.Map(_query);

        public FindSpecification<Contact> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Contact>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Contact> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Contact> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Contact> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Contact> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<Contact>(x => ids.Contains(x.Id));

            var clientIds = (from client in _query.For<Client>()
                             join contact in _query.For(specification) on client.Id equals contact.ClientId
                             select client.Id)
                .ToArray();

            var firmIds = (from firm in _query.For<Firm>()
                           join client in _query.For<Client>() on firm.ClientId equals client.Id
                           join contact in _query.For(specification) on client.Id equals contact.ClientId
                           select firm.Id)
                .ToArray();

            return clientIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Client), x))
                            .Concat(firmIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Firm), x)))
                            .ToArray();
        }
    }
}