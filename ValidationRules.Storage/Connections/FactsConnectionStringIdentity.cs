using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.ValidationRules.Storage.Connections
{
    public class FactsConnectionStringIdentity : IdentityBase<FactsConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id => 14;

        public override string Description => "Facts DB connection string";
    }
}