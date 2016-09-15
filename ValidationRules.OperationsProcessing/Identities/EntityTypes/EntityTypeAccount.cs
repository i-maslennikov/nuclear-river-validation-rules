using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeAccount : EntityTypeBase<EntityTypeAccount>
    {
        public override int Id { get; } = EntityTypeIds.Account;
        public override string Description { get; } = nameof(EntityTypeIds.Account);
    }
}