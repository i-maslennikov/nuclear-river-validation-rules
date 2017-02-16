using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeAdvertisementElementStatus : EntityTypeBase<EntityTypeAdvertisementElementStatus>
    {
        public override int Id { get; } = EntityTypeIds.AdvertisementElementStatus;
        public override string Description { get; } = nameof(EntityTypeIds.AdvertisementElementStatus);
    }
}