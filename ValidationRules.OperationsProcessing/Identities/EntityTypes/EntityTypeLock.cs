using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeLock : EntityTypeBase<EntityTypeLock>
    {
        public override int Id { get; } = EntityTypeIds.Lock;
        public override string Description { get; } = nameof(EntityTypeIds.Lock);
    }
}