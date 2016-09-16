using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeReleaseWithdrawal : EntityTypeBase<EntityTypeReleaseWithdrawal>
    {
        public override int Id { get; } = EntityTypeIds.ReleaseWithdrawal;
        public override string Description { get; } = nameof(EntityTypeIds.ReleaseWithdrawal);
    }
}