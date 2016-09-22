using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeCategory : EntityTypeBase<EntityTypeCategory>
    {
        public override int Id { get; } = EntityTypeIds.Category;
        public override string Description { get; } = nameof(EntityTypeIds.Category);
    }
}
