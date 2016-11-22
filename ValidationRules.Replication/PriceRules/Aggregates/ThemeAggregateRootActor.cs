using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.PriceRules.Aggregates
{
    public sealed class ThemeAggregateRootActor : EntityActorBase<Theme>, IAggregateRootActor
    {
        public ThemeAggregateRootActor(
            IQuery query,
            IBulkRepository<Theme> bulkRepository,
            IEqualityComparerFactory equalityComparerFactory)
            : base(query, bulkRepository, equalityComparerFactory, new ThemeAccessor(query))
        {
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => Array.Empty<IActor>();

        public sealed class ThemeAccessor : IStorageBasedDataObjectAccessor<Theme>
        {
            private readonly IQuery _query;

            public ThemeAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Theme> GetSource()
                => _query.For<Facts::Theme>().Select(x => new Theme { Id = x.Id, Name = x.Name });

            public FindSpecification<Theme> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Theme>(x => aggregateIds.Contains(x.Id));
            }
        }
    }
}