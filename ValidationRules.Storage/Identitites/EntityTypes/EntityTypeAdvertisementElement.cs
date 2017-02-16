using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeAdvertisementElement : EntityTypeBase<EntityTypeAdvertisementElement>
    {
        public override int Id { get; } = EntityTypeIds.AdvertisementElement;
        public override string Description { get; } = nameof(EntityTypeIds.AdvertisementElement);
    }
}
