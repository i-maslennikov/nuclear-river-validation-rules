using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.ProjectRules.Facts;

namespace NuClear.ValidationRules.Replication.ProjectRules.Aggregates
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

        public sealed class CategoryAccessor : AggregateDataChangesHandler<Category>, IStorageBasedDataObjectAccessor<Category>
        {
            private readonly IQuery _query;

            public CategoryAccessor(IQuery query)
            {
                Invalidate(MessageTypeCode.OrderMustUseCategoriesOnlyAvailableInProject);
                Invalidate(MessageTypeCode.OrderPositionCostPerClickMustBeSpecified);
                Invalidate(MessageTypeCode.OrderPositionCostPerClickMustNotBeLessMinimum);
                Invalidate(MessageTypeCode.OrderPositionSalesModelMustMatchCategorySalesModel);
                Invalidate(MessageTypeCode.ProjectMustContainCostPerClickMinimumRestriction);

                _query = query;
            }

            public IQueryable<Category> GetSource()
                => from category in _query.For<Facts::Category>()
                   select new Category { Id = category.Id, Name = category.Name };

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
