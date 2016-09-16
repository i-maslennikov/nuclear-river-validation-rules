using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypePricePosition : EntityTypeBase<EntityTypePricePosition>
    {
        public override int Id { get; } = EntityTypeIds.PricePosition;
        public override string Description { get; } = nameof(EntityTypeIds.PricePosition);
    }
}
