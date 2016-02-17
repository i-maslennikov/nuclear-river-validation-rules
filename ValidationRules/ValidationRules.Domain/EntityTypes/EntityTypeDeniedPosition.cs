using NuClear.AdvancedSearch.Common.Metadata;
using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Domain.EntityTypes
{
    public sealed class EntityTypeDeniedPosition : EntityTypeBase<EntityTypeDeniedPosition>
    {
        public override int Id { get; } = EntityTypeIds.DeniedPosition;
        public override string Description { get; } = nameof(EntityTypeIds.DeniedPosition);
    }
}
