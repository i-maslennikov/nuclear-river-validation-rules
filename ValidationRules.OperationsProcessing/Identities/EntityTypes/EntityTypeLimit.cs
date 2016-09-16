using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeLimit : EntityTypeBase<EntityTypeLimit>
    {
        public override int Id { get; } = EntityTypeIds.Limit;
        public override string Description { get; } = nameof(EntityTypeIds.Limit);
    }
}