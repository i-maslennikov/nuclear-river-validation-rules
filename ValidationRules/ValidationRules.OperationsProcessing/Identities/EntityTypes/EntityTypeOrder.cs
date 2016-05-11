using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeOrder : EntityTypeBase<EntityTypeOrder>
    {
        public override int Id { get; } = EntityTypeIds.Order;
        public override string Description { get; } = nameof(EntityTypeIds.Order);
    }
}
