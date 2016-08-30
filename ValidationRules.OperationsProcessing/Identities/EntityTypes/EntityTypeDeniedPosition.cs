using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeDeniedPosition : EntityTypeBase<EntityTypeDeniedPosition>
    {
        public override int Id { get; } = EntityTypeIds.DeniedPosition;
        public override string Description { get; } = nameof(EntityTypeIds.DeniedPosition);
    }
}
