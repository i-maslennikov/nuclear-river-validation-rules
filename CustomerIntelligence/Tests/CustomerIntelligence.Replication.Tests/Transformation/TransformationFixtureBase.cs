using NuClear.CustomerIntelligence.Domain.EntityTypes;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.Replication.Tests.Transformation
{
    internal abstract class TransformationFixtureBase : DataFixtureBase
    {
        protected static class Fact
        {
            public static SyncFactCommand Operation<T>(long entityId)
            {
                return new SyncFactCommand(typeof(T), entityId);
            }
        }

        protected static class Aggregate
        {
            public static AggregateOperation Initialize(IEntityType entityType, long entityId)
            {
                return new InitializeAggregate(new EntityReference(entityType, entityId));
            }

            public static AggregateOperation Recalculate(IEntityType entityType, long entityId)
            {
                return new RecalculateAggregate(new EntityReference(entityType, entityId));
            }

            public static AggregateOperation Destroy(IEntityType entityType, long entityId)
            {
                return new DestroyAggregate(new EntityReference(entityType, entityId));
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
                => new RecalculateAggregatePart(new EntityReference(EntityTypeProjectStatistics.Instance, projectId),
                                                new EntityReference(EntityTypeProjectCategoryStatistics.Instance, categoryId));

            private static IOperation ForProject(long projectId)
                => new RecalculateAggregate(new EntityReference(EntityTypeProjectStatistics.Instance, projectId));
        }
    }
}