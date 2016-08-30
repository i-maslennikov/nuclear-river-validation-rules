using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.ValidationRules.Storage.Identitites.Connections
{
    public class ErmConnectionStringIdentity : IdentityBase<ErmConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 15; }
        }

        public override string Description
        {
            get { return "Erm DB connnection string"; }
        }
    }
}