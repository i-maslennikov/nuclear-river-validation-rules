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
    public sealed class PositionAggregateRootActor : AggregateRootActor<Position>
    {
        public PositionAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Position> bulkRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new PositionAccessor(query), bulkRepository);
        }

        public sealed class PositionAccessor : AggregateDataChangesHandler<Position>, IStorageBasedDataObjectAccessor<Position>
        {
            private readonly IQuery _query;

            public PositionAccessor(IQuery query)
            {
                Invalidate(MessageTypeCode.FirmAddressMustBeLocatedOnTheMap);
                Invalidate(MessageTypeCode.OrderMustUseCategoriesOnlyAvailableInProject);
                Invalidate(MessageTypeCode.OrderPositionCostPerClickMustBeSpecified);
                Invalidate(MessageTypeCode.OrderPositionCostPerClickMustNotBeLessMinimum);
                Invalidate(MessageTypeCode.OrderPositionSalesModelMustMatchCategorySalesModel);

                _query = query;
            }

            public IQueryable<Position> GetSource()
                => from category in _query.For<Facts::Position>()
                   select new Position { Id = category.Id, Name = category.Name };

            public FindSpecification<Position> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Position>(x => aggregateIds.Contains(x.Id));
            }
        }
    }
}
