using System.Collections.Generic;
using System.Configuration;

using NuClear.River.Hosting.Common.Identities.Connections;
using NuClear.River.Hosting.Common.Settings;
using NuClear.Settings.API;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.ValidationRules.Storage.Connections;

namespace NuClear.ValidationRules.Querying.Host
{
    internal sealed class QueryingServiceSettings : SettingsContainerBase
    {
        public QueryingServiceSettings()
        {
            var connectionStringSettings = new ConnectionStringSettingsAspect(new Dictionary<IConnectionStringIdentity, string>
            {
                {
                    ErmConnectionStringIdentity.Instance,
                    ConfigurationManager.ConnectionStrings["Erm"].ConnectionString
                },
                {
                    AmsConnectionStringIdentity.Instance,
                    ConfigurationManager.ConnectionStrings["Ams"].ConnectionString
                },
                {
                    FactsConnectionStringIdentity.Instance,
                    ConfigurationManager.ConnectionStrings["Facts"].ConnectionString
                },
                {
                    MessagesConnectionStringIdentity.Instance,
                    ConfigurationManager.ConnectionStrings["Messages"].ConnectionString
                },
                {
                    LoggingConnectionStringIdentity.Instance,
                    ConfigurationManager.ConnectionStrings["Logging"].ConnectionString
                }
            });

            Aspects
                .Use(connectionStringSettings)
                .Use<EnvironmentSettingsAspect>();
        }
    }
}