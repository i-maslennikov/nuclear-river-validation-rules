using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata;

namespace NuClear.ValidationRules.Domain.EntityTypes
{
    public sealed class EntityTypeCategory : EntityTypeBase<EntityTypeCategory>
    {
        public override int Id { get; } = EntityTypeIds.Category;
        public override string Description { get; } = nameof(EntityTypeIds.Category);
    }
}