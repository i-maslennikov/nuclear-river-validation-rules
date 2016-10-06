using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeReleaseInfo : EntityTypeBase<EntityTypeReleaseInfo>
    {
        public override int Id { get; } = EntityTypeIds.ReleaseInfo;
        public override string Description { get; } = nameof(EntityTypeIds.ReleaseInfo);
    }
}