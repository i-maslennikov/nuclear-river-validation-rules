using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeRuleset : EntityTypeBase<EntityTypeRuleset>
    {
        public override int Id { get; } = EntityTypeIds.Ruleset;
        public override string Description { get; } = nameof(EntityTypeIds.Ruleset);
    }
}
