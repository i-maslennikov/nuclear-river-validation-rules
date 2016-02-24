using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata.Context;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.CustomerIntelligence.Replication.Tests.Transformation
{
    internal abstract class TransformationFixtureBase : DataFixtureBase
    {
        protected static class Fact
        {
            public static FactOperation Operation<T>(long entityId)
            {
                return new FactOperation(typeof(T), entityId);
            }
        }

        protected static class Aggregate
        {
            public static AggregateOperation Initialize(IEntityType entityType, long entityId)
            {
                return new InitializeAggregate(PredicateFactory.EntityById(entityType, entityId));
            }

            public static AggregateOperation Recalculate(IEntityType entityType, long entityId)
            {
                return new RecalculateAggregate(PredicateFactory.EntityById(entityType, entityId));
            }

            public static AggregateOperation Destroy(IEntityType entityType, long entityId)
            {
                return new DestroyAggregate(PredicateFactory.EntityById(entityType, entityId));
            }
        }

        protected static class Statistics
        {
            public static RecalculateStatisticsOperation Operation(long projectId, long? categoryId = null)
            {
                return new RecalculateStatisticsOperation(categoryId.HasValue
                    ? PredicateFactory.StatisticsByProjectAndCategory(projectId, categoryId.Value)
                    : PredicateFactory.StatisticsByProject(projectId));
            }
        }
    }
}