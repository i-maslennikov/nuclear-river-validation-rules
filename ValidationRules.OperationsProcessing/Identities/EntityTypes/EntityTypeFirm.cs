using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeFirm : EntityTypeBase<EntityTypeFirm>
    {
        public override int Id { get; } = EntityTypeIds.Firm;
        public override string Description { get; } = nameof(EntityTypeIds.Firm);
    }
}