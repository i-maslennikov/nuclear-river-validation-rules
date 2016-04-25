using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.CI;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Actors
{
    public sealed class ClientAggregateRootActor : EntityActorBase<Client>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<ClientContact> _clientContactBulkRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;

        public ClientAggregateRootActor(
            IQuery query,
            IBulkRepository<Client> clientBulkRepository,
            IBulkRepository<ClientContact> clientContactBulkRepository,
            IEqualityComparerFactory equalityComparerFactory)
            : base(query, clientBulkRepository, equalityComparerFactory, new ClientAccessor(query))
        {
            _query = query;
            _clientContactBulkRepository = clientContactBulkRepository;
            _equalityComparerFactory = equalityComparerFactory;
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors() => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors() => new IActor[]
            {
                new ValueObjectActor<ClientContact>(_query, _clientContactBulkRepository, _equalityComparerFactory, new ClientContactAccessor(_query))
            };

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