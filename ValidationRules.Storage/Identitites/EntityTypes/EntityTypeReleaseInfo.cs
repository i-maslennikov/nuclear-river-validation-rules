using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeReleaseInfo : EntityTypeBase<EntityTypeReleaseInfo>
    {
        public override int Id { get; } = EntityTypeIds.ReleaseInfo;
        public override string Description { get; } = nameof(EntityTypeIds.ReleaseInfo);
    }
}