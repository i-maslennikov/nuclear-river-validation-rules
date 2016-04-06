using NuClear.CustomerIntelligence.Domain.EntityTypes;
using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata.Model;
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
                return new InitializeAggregate(entityType, entityId);
            }

            public static AggregateOperation Recalculate(IEntityType entityType, long entityId)
            {
                return new RecalculateAggregate(entityType, entityId);
            }

            public static AggregateOperation Destroy(IEntityType entityType, long entityId)
            {
                return new DestroyAggregate(entityType, entityId);
            }
        }

        protected static class Statistics
        {
            public static IOperation Operation(long projectId, long? categoryId = null)
            {
                return categoryId.HasValue
                           ? ForProjectCategory(projectId, categoryId.Value)
                           : ForProject(projectId);
            }

            private static IOperation ForProjectCategory(long projectId, long categoryId)
                => new RecalculateAggregatePart(EntityTypeProjectStatistics.Instance, projectId, EntityTypeProjectCategoryStatistics.Instance, categoryId);

            private static IOperation ForProject(long projectId)
                => new RecalculateAggregate(EntityTypeProjectStatistics.Instance, projectId);
        }
    }
}