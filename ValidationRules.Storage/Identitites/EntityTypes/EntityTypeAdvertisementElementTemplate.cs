using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeAdvertisementElementTemplate : EntityTypeBase<EntityTypeAdvertisementElementTemplate>
    {
        public override int Id { get; } = EntityTypeIds.AdvertisementElementTemplate;
        public override string Description { get; } = nameof(EntityTypeIds.AdvertisementElementTemplate);
    }
}
