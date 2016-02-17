using NuClear.AdvancedSearch.Common.Metadata;
using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Domain.EntityTypes
{
    public sealed class EntityTypeOrderPositionAdvertisement : EntityTypeBase<EntityTypeOrderPositionAdvertisement>
    {
        public override int Id { get; } = EntityTypeIds.OrderPositionAdvertisement;
        public override string Description { get; } = nameof(EntityTypeIds.OrderPositionAdvertisement);
    }
}
