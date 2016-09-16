using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypePrice : EntityTypeBase<EntityTypePrice>
    {
        public override int Id { get; } = EntityTypeIds.Price;
        public override string Description { get; } = nameof(EntityTypeIds.Price);
    }
}
