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
    public sealed class ClientAccessor : IStorageBasedDataObjectAccessor<Client>, IDataChangesHandler<Client>
    {
        private readonly IQuery _query;

        public ClientAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Client> GetSource() => Specs.Map.Erm.ToFacts.Clients.Map(_query);

        public FindSpecification<Client> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Client>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Client> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Client), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Client> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Client), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Client> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Client), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Client> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<Client>(x => ids.Contains(x.Id));

            var firmIds = (from firm in _query.For<Firm>()
                           join client in _query.For(specification) on firm.ClientId equals client.Id
                           select firm.Id)
                .ToArray();

            return firmIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Firm), x))
                          .ToArray();
        }
    }
}