using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Model.CI;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API.Actors;
using NuClear.Replication.Core.API.DataObjects;
using NuClear.Replication.Core.API.Equality;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Actors
{
    public class CategoryGroupAggregateRootActor : EntityActorBase<CategoryGroup>, IAggregateRootActor
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
                => new FindSpecification<CategoryGroup>(x => commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().Contains(x.Id));
        }
    }
}