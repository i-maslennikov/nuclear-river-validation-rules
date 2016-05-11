using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.CI;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Replication.Actors
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
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Client>(x => aggregateIds.Contains(x.Id));
            }
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
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return Specs.Find.CI.ClientContacts(aggregateIds);
            }
        }
    }
}