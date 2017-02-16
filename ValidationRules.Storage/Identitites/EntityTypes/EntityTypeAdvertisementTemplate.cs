using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeAdvertisementTemplate : EntityTypeBase<EntityTypeAdvertisementTemplate>
    {
        public override int Id { get; } = EntityTypeIds.AdvertisementTemplate;
        public override string Description { get; } = nameof(EntityTypeIds.AdvertisementTemplate);
    }
}
