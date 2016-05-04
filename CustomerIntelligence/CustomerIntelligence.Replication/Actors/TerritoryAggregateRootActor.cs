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
    public sealed class TerritoryAggregateRootActor : EntityActorBase<Territory>, IAggregateRootActor
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
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Territory>(x => aggregateIds.Contains(x.Id));
            }
        }
    }
}