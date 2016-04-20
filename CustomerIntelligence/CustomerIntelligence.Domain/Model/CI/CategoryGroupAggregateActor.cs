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
    public class CategoryGroupAggregateActor : IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<CategoryGroup> _categoryGroupBulkRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;

        public CategoryGroupAggregateActor(
            IQuery query,
            IBulkRepository<CategoryGroup> categoryGroupBulkRepository,
            IEqualityComparerFactory equalityComparerFactory)
        {
            _query = query;
            _categoryGroupBulkRepository = categoryGroupBulkRepository;
            _equalityComparerFactory = equalityComparerFactory;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var actor = new CategoryGroupActor(_query, _categoryGroupBulkRepository, _equalityComparerFactory);
            return actor.ExecuteCommands(commands);
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors() => Array.Empty<IEntityActor>();

        public IReadOnlyCollection<IActor> GetValueObjectActors() => Array.Empty<IActor>();

        private sealed class CategoryGroupActor : AggregateRootActorBase<CategoryGroup>
        {
            public CategoryGroupActor(IQuery query,
                                      IBulkRepository<CategoryGroup> categoryGroupBulkRepository,
                                      IEqualityComparerFactory equalityComparerFactory)
                : base(query, categoryGroupBulkRepository, equalityComparerFactory, new CategoryGroupAccessor(query))
            {
            }
        }

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