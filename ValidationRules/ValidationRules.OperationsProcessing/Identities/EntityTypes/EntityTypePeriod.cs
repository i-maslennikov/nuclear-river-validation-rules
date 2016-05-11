using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypePeriod : EntityTypeBase<EntityTypePeriod>
    {
        public override int Id { get; } = EntityTypeIds.Period;
        public override string Description { get; } = nameof(EntityTypeIds.Period);
    }
}
