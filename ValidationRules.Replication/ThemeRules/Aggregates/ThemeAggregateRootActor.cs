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
using NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.ThemeRules.Aggregates
{
    public sealed class ThemeAggregateRootActor : EntityActorBase<Theme>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IEqualityComparerFactory _equalityComparerFactory;
        private readonly IBulkRepository<Theme.InvalidCategory> _invalidCategoryBulkRepository;

        public ThemeAggregateRootActor(
            IQuery query,
            IBulkRepository<Theme> bulkRepository,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Theme.InvalidCategory> invalidCategoryBulkRepository)
            : base(query, bulkRepository, equalityComparerFactory, new ThemeAccessor(query))
        {
            _query = query;
            _equalityComparerFactory = equalityComparerFactory;
            _invalidCategoryBulkRepository = invalidCategoryBulkRepository;
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<Theme.InvalidCategory>(_query, _invalidCategoryBulkRepository, _equalityComparerFactory, new InvalidCategoryAccessor(_query)),
                };

        public sealed class ThemeAccessor : IStorageBasedDataObjectAccessor<Theme>
        {
            private readonly IQuery _query;

            public ThemeAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Theme> GetSource()
                => from theme in _query.For<Facts::Theme>()
                   select new Theme
                   {
                       Id = theme.Id,
                       Name = theme.Name,
                       BeginDistribution = theme.BeginDistribution,
                       EndDistribution = theme.EndDistribution,
                       IsDefault = theme.IsDefault,
                   };

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

        public sealed class InvalidCategoryAccessor : IStorageBasedDataObjectAccessor<Theme.InvalidCategory>
        {
            private readonly IQuery _query;

            public InvalidCategoryAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Theme.InvalidCategory> GetSource()
            {
                var invalidCategories =
                    from themeCategory in _query.For<Facts::ThemeCategory>()
                    from category in _query.For<Facts::Category>().Where(x => x.Id == themeCategory.CategoryId)
                    where !category.IsActiveNotDeleted // рубрика невалидна
                    select new Theme.InvalidCategory
                    {
                        ThemeId = themeCategory.ThemeId,
                        CategoryId = category.Id,
                    };

                return invalidCategories;
            }

            public FindSpecification<Theme.InvalidCategory> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Theme.InvalidCategory>(x => aggregateIds.Contains(x.ThemeId));
            }
        }
    }
}
