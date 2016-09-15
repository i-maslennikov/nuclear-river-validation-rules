using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeBargain : EntityTypeBase<EntityTypeBargain>
    {
        public override int Id { get; } = EntityTypeIds.Bargain;
        public override string Description { get; } = nameof(EntityTypeIds.Bargain);
    }
}