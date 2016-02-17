using NuClear.AdvancedSearch.Common.Metadata;
using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Domain.EntityTypes
{
    public sealed class EntityTypeOrderPosition : EntityTypeBase<EntityTypeOrderPosition>
    {
        public override int Id { get; } = EntityTypeIds.OrderPosition;
        public override string Description { get; } = nameof(EntityTypeIds.OrderPosition);
    }
}
