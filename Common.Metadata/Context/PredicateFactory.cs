using System.Collections.Generic;

using NuClear.Model.Common.Entities;

namespace NuClear.River.Common.Metadata.Context
{
    public static class PredicateFactory
    {
        public static Predicate EntityById(IEntityType entityType, long id)
        {
            var properties = new Dictionary<string, string>();
            PredicateProperty.Type.SetValue(properties, Type.ById);
            PredicateProperty.EntityId.SetValue(properties, id);
            PredicateProperty.EntityType.SetValue(properties, entityType.Id);

            return new Predicate(properties, new Predicate[0]);
        }

        public static Predicate StatisticsByProject(long projectId)
        {
            var properties = new Dictionary<string, string>();
            PredicateProperty.Type.SetValue(properties, Type.ByProject);
            PredicateProperty.ProjectId.SetValue(properties, projectId);

            return new Predicate(properties, new Predicate[0]);
        }

        public static Predicate StatisticsByProjectAndCategory(long projectId, long categoryId)
        {
            var properties = new Dictionary<string, string>();
            PredicateProperty.Type.SetValue(properties, Type.ByProjectCategory);
            PredicateProperty.ProjectId.SetValue(properties, projectId);
            PredicateProperty.CategoryId.SetValue(properties, categoryId);

            return new Predicate(properties, new Predicate[0]);
        }

        // todo: можно подумать о "PredicateIdentity" и вынести туда
        public static class Type
        {
            public const string ById = "byId";
            public const string ByProject = "byProject";
            public const string ByProjectCategory = "byProjectCategory";
        }
    }
}