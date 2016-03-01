using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata;

namespace NuClear.ValidationRules.Domain.EntityTypes
{
    public sealed class EntityTypeGlobalDeniedPosition : EntityTypeBase<EntityTypeGlobalDeniedPosition>
    {
        public override int Id { get; } = EntityTypeIds.GlobalDeniedPosition;
        public override string Description { get; } = nameof(EntityTypeIds.GlobalDeniedPosition);
    }
}
