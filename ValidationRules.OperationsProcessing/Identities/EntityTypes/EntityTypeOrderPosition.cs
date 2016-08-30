using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeOrderPosition : EntityTypeBase<EntityTypeOrderPosition>
    {
        public override int Id { get; } = EntityTypeIds.OrderPosition;
        public override string Description { get; } = nameof(EntityTypeIds.OrderPosition);
    }
}
