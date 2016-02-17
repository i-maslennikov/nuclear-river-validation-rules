using NuClear.AdvancedSearch.Common.Metadata;
using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Domain.EntityTypes
{
    public sealed class EntityTypeCategory : EntityTypeBase<EntityTypeCategory>
    {
        public override int Id { get; } = EntityTypeIds.Category;
        public override string Description { get; } = nameof(EntityTypeIds.Category);
    }
}
