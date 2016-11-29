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
    public sealed class CategoryAggregateRootActor : AggregateRootActor<Category>
    {
        public CategoryAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Category> bulkRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new CategoryAccessor(query), bulkRepository);
        }

        public sealed class CategoryAccessor : DataChangesHandler<Category>, IStorageBasedDataObjectAccessor<Category>
        {
            private readonly IQuery _query;

            public CategoryAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.ThemeCategoryMustBeActiveAndNotDeleted,
                    };

            public IQueryable<Category> GetSource()
                => from category in _query.For<Facts::Category>()
                   select new Category
                       {
                           Id = category.Id,
                           Name = category.Name,
                       };

            public FindSpecification<Category> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Category>(x => aggregateIds.Contains(x.Id));
            }
        }
    }
}
