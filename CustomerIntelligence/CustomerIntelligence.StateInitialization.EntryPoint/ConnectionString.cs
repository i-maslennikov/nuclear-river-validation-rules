using NuClear.CustomerIntelligence.Storage.Identitites.Connections;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.CustomerIntelligence.StateInitialization.EntryPoint
{
    internal sealed class ConnectionString
    {
        public static IConnectionStringIdentity Erm =
            ErmConnectionStringIdentity.Instance;

        public static IConnectionStringIdentity Facts =
            FactsConnectionStringIdentity.Instance;

        public static IConnectionStringIdentity CustomerIntelligence =
            CustomerIntelligenceConnectionStringIdentity.Instance;
    }
}