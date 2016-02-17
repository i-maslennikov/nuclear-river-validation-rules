using NuClear.AdvancedSearch.Common.Metadata;
using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Domain.EntityTypes
{
    public sealed class EntityTypePosition : EntityTypeBase<EntityTypePosition>
    {
        public override int Id { get; } = EntityTypeIds.Position;
        public override string Description { get; } = nameof(EntityTypeIds.Position);
    }
}
