using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Events;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Accessors
{
    public sealed class ClientAccessor : IStorageBasedDataObjectAccessor<Client>, IDataChangesHandler<Client>
    {
        private readonly IQuery _query;

        public ClientAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Client> GetSource() => Specs.Map.Erm.ToFacts.Clients.Map(_query);

        public FindSpecification<Client> GetFindSpecification(IReadOnlyCollection<ICommand> commands) =>
            new FindSpecification<Client>(x => commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).Contains(x.Id));

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

            return Specs.Map.Facts.ToFirmAggregate.ByClient(specification)
                        .Map(_query)
                        .Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Firm), x))
                        .ToArray();
        }
    }
}