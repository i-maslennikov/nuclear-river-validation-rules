using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeAssociatedPositionsGroup : EntityTypeBase<EntityTypeAssociatedPositionsGroup>
    {
        public override int Id { get; } = EntityTypeIds.AssociatedPositionsGroup;
        public override string Description { get; } = nameof(EntityTypeIds.AssociatedPositionsGroup);
    }
}
