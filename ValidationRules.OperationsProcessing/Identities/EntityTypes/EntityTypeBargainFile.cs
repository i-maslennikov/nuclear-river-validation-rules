using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeBargainFile : EntityTypeBase<EntityTypeBargainFile>
    {
        public override int Id { get; } = EntityTypeIds.BargainFile;
        public override string Description { get; } = nameof(EntityTypeIds.BargainFile);
    }
}