using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.River.Hosting.Common.Identities.Connections
{
    public class LoggingConnectionStringIdentity : IdentityBase<LoggingConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id => 7;

        public override string Description => "Logging storage connection string identity";
    }
}