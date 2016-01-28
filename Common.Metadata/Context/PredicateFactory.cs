using System.Collections.Generic;

using NuClear.Model.Common.Entities;

namespace NuClear.AdvancedSearch.Common.Metadata.Context
{
    public static class PredicateFactory
    {
        public static Predicate EntityById(IEntityType entityType, long id)
        {
            var properties = new Dictionary<string, string>();
            PredicateProperty.Id.SetValue(properties, "byId");
            PredicateProperty.EntityId.SetValue(properties, id);
            PredicateProperty.EntityType.SetValue(properties, entityType.Id);

            return new Predicate(properties, new Predicate[0]);
        }

        public static Predicate StatisticsByProject(long projectId)
        {
            var properties = new Dictionary<string, string>();
            PredicateProperty.Id.SetValue(properties, "byProjectReference");
            PredicateProperty.ProjectId.SetValue(properties, projectId);

            return new Predicate(properties, new Predicate[0]);
        }

        public static Predicate StatisticsByProjectAndCategory(long projectId, long categoryId)
        {
            var properties = new Dictionary<string, string>();
            PredicateProperty.Id.SetValue(properties, "byProjectCategoryReference");
            PredicateProperty.ProjectId.SetValue(properties, projectId);
            PredicateProperty.CategoryId.SetValue(properties, categoryId);

            return new Predicate(properties, new Predicate[0]);
        }
    }
}