using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeTheme : EntityTypeBase<EntityTypeTheme>
    {
        public override int Id { get; } = EntityTypeIds.Theme;
        public override string Description { get; } = nameof(EntityTypeIds.Theme);
    }
}