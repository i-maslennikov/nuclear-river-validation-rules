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
    public sealed class CategoryGroupAggregateRootActor : EntityActorBase<CategoryGroup>, IAggregateRootActor
    {
        public CategoryGroupAggregateRootActor(
            IQuery query,
            IBulkRepository<CategoryGroup> categoryGroupBulkRepository,
            IEqualityComparerFactory equalityComparerFactory)
            : base(query, categoryGroupBulkRepository, equalityComparerFactory, new CategoryGroupAccessor(query))
        {
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors() => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors() => Array.Empty<IActor>();

        public sealed class CategoryGroupAccessor : IStorageBasedDataObjectAccessor<CategoryGroup>
        {
            private readonly IQuery _query;

            public CategoryGroupAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<CategoryGroup> GetSource() => Specs.Map.Facts.ToCI.CategoryGroups.Map(_query);

            public FindSpecification<CategoryGroup> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<CategoryGroup>(x => aggregateIds.Contains(x.Id));
            }
        }
    }
}