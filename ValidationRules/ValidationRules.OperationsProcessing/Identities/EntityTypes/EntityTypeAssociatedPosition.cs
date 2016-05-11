using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeAssociatedPosition : EntityTypeBase<EntityTypeAssociatedPosition>
    {
        public override int Id { get; } = EntityTypeIds.AssociatedPosition;
        public override string Description { get; } = nameof(EntityTypeIds.AssociatedPosition);
    }
}
