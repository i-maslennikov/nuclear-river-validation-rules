using NuClear.AdvancedSearch.Common.Metadata;
using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeAssociatedPositionsGroup : EntityTypeBase<EntityTypeAssociatedPositionsGroup>
    {
        public override int Id { get; } = EntityTypeIds.AssociatedPositionsGroup;
        public override string Description { get; } = nameof(EntityTypeIds.AssociatedPositionsGroup);
    }
}
