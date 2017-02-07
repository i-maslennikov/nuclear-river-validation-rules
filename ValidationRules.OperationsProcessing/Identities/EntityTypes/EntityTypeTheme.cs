using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Storage;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeTheme : EntityTypeBase<EntityTypeTheme>
    {
        public override int Id { get; } = EntityTypeIds.Theme;
        public override string Description { get; } = nameof(EntityTypeIds.Theme);
    }
}