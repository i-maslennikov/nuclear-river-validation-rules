using NuClear.River.Hosting.Common.Identities.Connections;
using NuClear.River.Hosting.Common.Settings;
using NuClear.Settings.API;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.ValidationRules.Storage.Connections;

using ValidationRules.Hosting.Common.Settings.Connections;

namespace NuClear.ValidationRules.Querying.Host
{
    internal sealed class QueryingServiceSettings : SettingsContainerBase
    {
        public QueryingServiceSettings()
        {
            var connectionString = ConnectionStrings.For(ErmConnectionStringIdentity.Instance,
                                                         AmsConnectionStringIdentity.Instance,
                                                         FactsConnectionStringIdentity.Instance,
                                                         MessagesConnectionStringIdentity.Instance,
                                                         LoggingConnectionStringIdentity.Instance);

            Aspects
                .Use(new ConnectionStringSettingsAspect(connectionString))
                .Use<EnvironmentSettingsAspect>();
        }
    }
}