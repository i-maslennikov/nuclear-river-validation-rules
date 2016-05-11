using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.StateInitialization.Tests.Identitites.Connections;
using NuClear.CustomerIntelligence.Storage.Identitites.Connections;
using NuClear.DataTest.Metamodel;
using NuClear.DataTest.Metamodel.Dsl;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    public sealed class MappedConnectionStringSettings : IConnectionStringSettings
    {
        private static readonly IReadOnlyDictionary<IConnectionStringIdentity, IConnectionStringIdentity> Mapping =
            new Dictionary<IConnectionStringIdentity, IConnectionStringIdentity>
            {
                { ErmTestConnectionStringIdentity.Instance, ErmConnectionStringIdentity.Instance },
                { FactsTestConnectionStringIdentity.Instance, FactsConnectionStringIdentity.Instance },
                { BitTestConnectionStringIdentity.Instance, FactsConnectionStringIdentity.Instance },
                { CustomerIntelligenceTestConnectionStringIdentity.Instance, CustomerIntelligenceConnectionStringIdentity.Instance },
            };

        private readonly IConnectionStringSettings _connectionStringSettings;
        private readonly IReadOnlyDictionary<IConnectionStringIdentity, IConnectionStringIdentity> _mapping;

        public MappedConnectionStringSettings(
            IConnectionStringSettings connectionStringSettings,
            IReadOnlyDictionary<IConnectionStringIdentity, IConnectionStringIdentity> mapping)
        {
            _connectionStringSettings = connectionStringSettings;
            _mapping = mapping;
        }

        public static IConnectionStringSettings CreateMappedSettings(
            IConnectionStringSettings baseSettings,
            ActMetadataElement actMetadata,
            IDictionary<string, SchemaMetadataElement> schemaMetadata)
        {
            var mapping = new Dictionary<IConnectionStringIdentity, IConnectionStringIdentity>();

            foreach (var context in new[] { actMetadata.Source, actMetadata.Target })
            {
                var contextDbConnection = schemaMetadata[context].ConnectionStringIdentity;
                var bulktoolDbConnection = Mapping[contextDbConnection];
                mapping.Add(bulktoolDbConnection, contextDbConnection);
            }

            return new MappedConnectionStringSettings(baseSettings, mapping);
        }

        public string GetConnectionString(IConnectionStringIdentity identity)
        {
            return _connectionStringSettings.GetConnectionString(_mapping[identity]);
        }

        public IReadOnlyDictionary<IConnectionStringIdentity, string> AllConnectionStrings
            => _connectionStringSettings.AllConnectionStrings.ToDictionary(x => _mapping[x.Key], x => x.Value);
    }
}