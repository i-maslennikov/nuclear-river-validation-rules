using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

using NuClear.Storage.API.ConnectionStrings;
using NuClear.ValidationRules.Replication.StateInitialization.Tests.Identitites.Connections;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed class RunnerConnectionStringSettings : ConnectionStringSettingsAspect
    {
        private static readonly string DatabaseNamePostfix = Guid.NewGuid().ToString("N");

        public RunnerConnectionStringSettings()
            : base(CreateConnectionStringMappings(GetTestAssemblyConnectionStrings()))
        {
        }

        private static ConnectionStringSettingsCollection GetTestAssemblyConnectionStrings()
        {
            var assemblyLocation = typeof(RunnerConnectionStringSettings).Assembly.Location;
            return ConfigurationManager.OpenExeConfiguration(assemblyLocation).ConnectionStrings.ConnectionStrings;
        }

        private static IReadOnlyDictionary<IConnectionStringIdentity, string> CreateConnectionStringMappings(ConnectionStringSettingsCollection configuration)
            => new Dictionary<IConnectionStringIdentity, string>
                {
                    { ErmTestConnectionStringIdentity.Instance, MakeUniqueSqlConnectionString(configuration, ConnectionStringName.Erm) },
                    { FactsTestConnectionStringIdentity.Instance, MakeUniqueSqlConnectionString(configuration, ConnectionStringName.Facts) },
                    { AggregatesTestConnectionStringIdentity.Instance, MakeUniqueSqlConnectionString(configuration, ConnectionStringName.Aggregates) },
                    { MessagesTestConnectionStringIdentity.Instance, MakeUniqueSqlConnectionString(configuration, ConnectionStringName.Messages) },
                };

        private static string MakeUniqueSqlConnectionString(ConnectionStringSettingsCollection configuration, string connectionName)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var connectionString = configuration[connectionName];

            if (connectionString == null)
            {
                throw new ArgumentException($"missing connection string {connectionName}", nameof(configuration));
            }

            var builder = new SqlConnectionStringBuilder(connectionString.ConnectionString);
            builder.InitialCatalog = builder.InitialCatalog + DatabaseNamePostfix;
            return builder.ToString();
        }
    }
}