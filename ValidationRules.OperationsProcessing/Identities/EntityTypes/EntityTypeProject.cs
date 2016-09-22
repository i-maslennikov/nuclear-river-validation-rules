using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeProject : EntityTypeBase<EntityTypeProject>
    {
        public override int Id { get; } = EntityTypeIds.Project;
        public override string Description { get; } = nameof(EntityTypeIds.Project);
    }
}
