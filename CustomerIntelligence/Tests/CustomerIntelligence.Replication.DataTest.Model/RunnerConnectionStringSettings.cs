using System.Collections.Generic;
using System.Configuration;

using NuClear.CustomerIntelligence.Replication.StateInitialization.Tests.Identitites.Connections;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    public sealed class RunnerConnectionStringSettings : ConnectionStringSettingsAspect
    {
        public RunnerConnectionStringSettings()
            : base(CreateConnectionStringMappings(GetTestAssemblyConnectionStrings()))
        {
        }

        private static ConnectionStringSettingsCollection GetTestAssemblyConnectionStrings()
        {
            var assemblyLocation = typeof(RunnerConnectionStringSettings).Assembly.Location;
            return ConfigurationManager.OpenExeConfiguration(assemblyLocation).ConnectionStrings.ConnectionStrings;
        }

        private static IReadOnlyDictionary<IConnectionStringIdentity, string> CreateConnectionStringMappings(ConnectionStringSettingsCollection configuration) =>
            new Dictionary<IConnectionStringIdentity, string>
            {
                { ErmTestConnectionStringIdentity.Instance, configuration[ConnectionStringName.Erm].ConnectionString.ProtectSqlConnectionString() },
                { FactsTestConnectionStringIdentity.Instance, configuration[ConnectionStringName.Facts].ConnectionString.ProtectSqlConnectionString() },
                { CustomerIntelligenceTestConnectionStringIdentity.Instance, configuration[ConnectionStringName.CustomerIntelligence].ConnectionString.ProtectSqlConnectionString() },
                { BitTestConnectionStringIdentity.Instance, configuration[ConnectionStringName.Bit].ConnectionString.ProtectSqlConnectionString() },
                { StatisticsTestConnectionStringIdentity.Instance, configuration[ConnectionStringName.Statistics].ConnectionString.ProtectSqlConnectionString() },

                { ErmMasterConnectionStringIdentity.Instance, configuration[ConnectionStringName.ErmMaster].ConnectionString },
            };
    }
}