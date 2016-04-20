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
    public class TerritoryAggregateActor : IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<Territory> _territoryBulkRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;

        public TerritoryAggregateActor(
            IQuery query,
            IBulkRepository<Territory> territoryBulkRepository,
            IEqualityComparerFactory equalityComparerFactory)
        {
            _query = query;
            _territoryBulkRepository = territoryBulkRepository;
            _equalityComparerFactory = equalityComparerFactory;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var actor = new TerritoryActor(_query, _territoryBulkRepository, _equalityComparerFactory);
            return actor.ExecuteCommands(commands);
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors() => Array.Empty<IEntityActor>();


        public IReadOnlyCollection<IActor> GetValueObjectActors() => Array.Empty<IActor>();

        private sealed class TerritoryActor : AggregateRootActorBase<Territory>
        {
            public TerritoryActor(IQuery query,
                                  IBulkRepository<Territory> territoryBulkRepository,
                                  IEqualityComparerFactory equalityComparerFactory)
                : base(query, territoryBulkRepository, equalityComparerFactory, new TerritoryAccessor(query))
            {
            }
        }

        public sealed class TerritoryAccessor : IStorageBasedDataObjectAccessor<Territory>
        {
            private readonly IQuery _query;

            public TerritoryAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Territory> GetSource() => Specs.Map.Facts.ToCI.Territories.Map(_query);

            public FindSpecification<Territory> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => new FindSpecification<Territory>(x => commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().Contains(x.Id));
        }
    }
}