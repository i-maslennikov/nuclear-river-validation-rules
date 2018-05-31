using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.ValidationRules.Storage.Connections
{
    public sealed class RulesetConnectionStringIdentity : IdentityBase<RulesetConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id => 20;

        public override string Description => "Ruleset connection string identity";
    }
}