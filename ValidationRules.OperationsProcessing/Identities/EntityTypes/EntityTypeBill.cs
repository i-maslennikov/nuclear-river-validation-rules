using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeBill : EntityTypeBase<EntityTypeBill>
    {
        public override int Id { get; } = EntityTypeIds.Bill;
        public override string Description { get; } = nameof(EntityTypeIds.Bill);
    }
}