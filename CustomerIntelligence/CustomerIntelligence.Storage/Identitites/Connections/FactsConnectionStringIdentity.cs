using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.CustomerIntelligence.Storage.Identitites.Connections
{
    public class FactsConnectionStringIdentity : IdentityBase<FactsConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id => 2;

        public override string Description => "Facts DB connection string";
    }
}