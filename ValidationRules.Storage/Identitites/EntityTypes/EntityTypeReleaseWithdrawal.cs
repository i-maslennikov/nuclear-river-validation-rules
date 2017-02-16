using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeReleaseWithdrawal : EntityTypeBase<EntityTypeReleaseWithdrawal>
    {
        public override int Id { get; } = EntityTypeIds.ReleaseWithdrawal;
        public override string Description { get; } = nameof(EntityTypeIds.ReleaseWithdrawal);
    }
}