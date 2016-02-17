using NuClear.AdvancedSearch.Common.Metadata;
using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Domain.EntityTypes
{
    public sealed class EntityTypePricePosition : EntityTypeBase<EntityTypePricePosition>
    {
        public override int Id { get; } = EntityTypeIds.PricePosition;
        public override string Description { get; } = nameof(EntityTypeIds.PricePosition);
    }
}
