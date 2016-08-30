using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.ValidationRules.Storage.Identitites.Connections
{
    public class FactsConnectionStringIdentity : IdentityBase<FactsConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 14; }
        }

        public override string Description
        {
            get { return "Facts DB connection string"; }
        }
    }
}