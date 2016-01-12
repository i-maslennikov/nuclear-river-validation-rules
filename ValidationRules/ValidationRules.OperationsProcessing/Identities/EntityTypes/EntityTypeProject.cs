using NuClear.AdvancedSearch.Common.Metadata;
using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeProject : EntityTypeBase<EntityTypeProject>
    {
        public override int Id { get; } = EntityTypeIds.Project;
        public override string Description { get; } = nameof(EntityTypeIds.Project);
    }
}
