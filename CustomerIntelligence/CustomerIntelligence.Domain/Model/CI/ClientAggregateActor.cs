using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.Replication.Core.API.Equality;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Model.CI
{
    public sealed class ClientAggregateActor : IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<Client> _clientBulkRepository;
        private readonly IBulkRepository<ClientContact> _clientContactBulkRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;

        public ClientAggregateActor(
            IQuery query,
            IBulkRepository<Client> clientBulkRepository,
            IBulkRepository<ClientContact> clientContactBulkRepository,
            IEqualityComparerFactory equalityComparerFactory)
        {
            _query = query;
            _clientBulkRepository = clientBulkRepository;
            _clientContactBulkRepository = clientContactBulkRepository;
            _equalityComparerFactory = equalityComparerFactory;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var actor = new ClientActor(_query, _clientBulkRepository, _equalityComparerFactory);
            return actor.ExecuteCommands(commands);
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors() => Array.Empty<IEntityActor>();

        public IReadOnlyCollection<IActor> GetValueObjectActors() => new IActor[]
            {
                new ReplaceStorageBasedDataObjectsActor<ClientContact>(
                    _query,
                    _clientContactBulkRepository,
                    _equalityComparerFactory,
                    new ClientContactAccessor(_query))
            };

        private sealed class ClientActor : AggregateRootActorBase<Client>
        {
            public ClientActor(IQuery query,
                               IBulkRepository<Client> clientBulkRepository,
                               IEqualityComparerFactory equalityComparerFactory)
                : base(query, clientBulkRepository, equalityComparerFactory, new ClientAccessor(query))
            {
            }
        }

        public sealed class ClientAccessor : IStorageBasedDataObjectAccessor<Client>
        {
            private readonly IQuery _query;

            public ClientAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Client> GetSource() => Specs.Map.Facts.ToCI.Clients.Map(_query);

            public FindSpecification<Client> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => new FindSpecification<Client>(x => commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().Contains(x.Id));
        }

        public sealed class ClientContactAccessor : IStorageBasedDataObjectAccessor<ClientContact>
        {
            private readonly IQuery _query;

            public ClientContactAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<ClientContact> GetSource() => Specs.Map.Facts.ToCI.ClientContacts.Map(_query);

            public FindSpecification<ClientContact> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => Specs.Find.CI.ClientContacts(commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().ToArray());
        }
    }
}