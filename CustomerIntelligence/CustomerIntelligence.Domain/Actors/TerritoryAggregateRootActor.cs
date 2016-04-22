using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Model.CI;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Actors;
using NuClear.Replication.Core.API.DataObjects;
using NuClear.Replication.Core.API.Equality;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Actors
{
    public class TerritoryAggregateRootActor : EntityActorBase<Territory>, IAggregateRootActor
    {
        public TerritoryAggregateRootActor(
            IQuery query,
            IBulkRepository<Territory> territoryBulkRepository,
            IEqualityComparerFactory equalityComparerFactory)
            : base(query, territoryBulkRepository, equalityComparerFactory, new TerritoryAccessor(query))
        {
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors() => Array.Empty<IEntityActor>();


        public override IReadOnlyCollection<IActor> GetValueObjectActors() => Array.Empty<IActor>();

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