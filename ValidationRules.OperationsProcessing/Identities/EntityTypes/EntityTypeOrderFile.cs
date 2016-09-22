using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeOrderFile : EntityTypeBase<EntityTypeOrderFile>
    {
        public override int Id { get; } = EntityTypeIds.OrderFile;
        public override string Description { get; } = nameof(EntityTypeIds.OrderFile);
    }
}