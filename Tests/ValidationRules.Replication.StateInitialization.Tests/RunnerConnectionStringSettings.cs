using System.Collections.Generic;
using System.Configuration;

using NuClear.Storage.API.ConnectionStrings;
using NuClear.ValidationRules.Storage.Connections;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed class RunnerConnectionStringSettings : ConnectionStringSettingsAspect
    {
        public RunnerConnectionStringSettings()
            : base(CreateConnectionStringMappings())
        {
        }

        private static IReadOnlyDictionary<IConnectionStringIdentity, string> CreateConnectionStringMappings()
            => new Dictionary<IConnectionStringIdentity, string>
                {
                    { ErmConnectionStringIdentity.Instance, ConfigurationManager.ConnectionStrings["Erm"].ConnectionString },
                    { FactsConnectionStringIdentity.Instance, ConfigurationManager.ConnectionStrings["Facts"].ConnectionString },
                    { AggregatesConnectionStringIdentity.Instance, ConfigurationManager.ConnectionStrings["Aggregates"].ConnectionString },
                    { MessagesConnectionStringIdentity.Instance, ConfigurationManager.ConnectionStrings["Messages"].ConnectionString }
                };
    }
}