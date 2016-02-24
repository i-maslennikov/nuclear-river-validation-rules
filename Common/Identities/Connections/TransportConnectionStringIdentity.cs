using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.River.Common.Identities.Connections
{
    public class TransportConnectionStringIdentity : IdentityBase<TransportConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id => 3;

        public override string Description => "Operations transport DB connection string identity";
    }
}