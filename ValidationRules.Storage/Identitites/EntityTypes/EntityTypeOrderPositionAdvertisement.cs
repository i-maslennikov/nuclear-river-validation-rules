using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeOrderPositionAdvertisement : EntityTypeBase<EntityTypeOrderPositionAdvertisement>
    {
        public override int Id { get; } = EntityTypeIds.OrderPositionAdvertisement;
        public override string Description { get; } = nameof(EntityTypeIds.OrderPositionAdvertisement);
    }
}
