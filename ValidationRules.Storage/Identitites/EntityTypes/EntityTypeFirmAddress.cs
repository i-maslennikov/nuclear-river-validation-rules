using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeFirmAddress : EntityTypeBase<EntityTypeFirmAddress>
    {
        public override int Id { get; } = EntityTypeIds.FirmAddress;
        public override string Description { get; } = nameof(EntityTypeIds.FirmAddress);
    }
}