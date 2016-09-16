using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeRulesetRule : EntityTypeBase<EntityTypeRulesetRule>
    {
        public override int Id { get; } = EntityTypeIds.RulesetRule;
        public override string Description { get; } = nameof(EntityTypeIds.RulesetRule);
    }
}