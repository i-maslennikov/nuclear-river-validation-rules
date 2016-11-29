using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.ThemeRules.Facts;

namespace NuClear.ValidationRules.Replication.ThemeRules.Aggregates
{
    public sealed class ThemeAggregateRootActor : AggregateRootActor<Theme>
    {
        public ThemeAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Theme> bulkRepository,
            IBulkRepository<Theme.InvalidCategory> invalidCategoryBulkRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new ThemeAccessor(query), bulkRepository,
               HasValueObject(new InvalidCategoryAccessor(query), invalidCategoryBulkRepository));
        }

        public sealed class ThemeAccessor : DataChangesHandler<Theme>, IStorageBasedDataObjectAccessor<Theme>
        {
            private readonly IQuery _query;

            public ThemeAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.DefaultThemeMustHaveOnlySelfAds,
                        MessageTypeCode.ThemeCategoryMustBeActiveAndNotDeleted,
                        MessageTypeCode.ThemePeriodMustContainOrderPeriod,
                    };

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

        public sealed class InvalidCategoryAccessor : DataChangesHandler<Theme.InvalidCategory>, IStorageBasedDataObjectAccessor<Theme.InvalidCategory>
        {
            private readonly IQuery _query;

            public InvalidCategoryAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.ThemeCategoryMustBeActiveAndNotDeleted,
                    };

            public IQueryable<Theme.InvalidCategory> GetSource()
            {
                var invalidCategories =
                    from themeCategory in _query.For<Facts::ThemeCategory>()
                    from category in _query.For<Facts::Category>().Where(x => x.Id == themeCategory.CategoryId)
                    where category.IsNotActiveOrDeleted // рубрика невалидна
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
