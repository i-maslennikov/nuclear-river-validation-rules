using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.CustomerIntelligence.Storage.Identitites.Connections
{
    public class ErmConnectionStringIdentity : IdentityBase<ErmConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id => 1;

        public override string Description => "Erm DB connnection string";
    }
}