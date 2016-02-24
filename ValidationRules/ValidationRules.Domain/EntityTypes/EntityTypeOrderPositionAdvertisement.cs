using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata;

namespace NuClear.ValidationRules.Domain.EntityTypes
{
    public sealed class EntityTypeOrderPositionAdvertisement : EntityTypeBase<EntityTypeOrderPositionAdvertisement>
    {
        public override int Id { get; } = EntityTypeIds.OrderPositionAdvertisement;
        public override string Description { get; } = nameof(EntityTypeIds.OrderPositionAdvertisement);
    }
}
