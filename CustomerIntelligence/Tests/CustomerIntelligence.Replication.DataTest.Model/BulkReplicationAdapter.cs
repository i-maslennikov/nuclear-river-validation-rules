using System.Linq;

using NuClear.CustomerIntelligence.StateInitialization.EntryPoint;
using NuClear.DataTest.Metamodel;
using NuClear.DataTest.Metamodel.Dsl;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Replication.Bulk.API;
using NuClear.Replication.Bulk.API.Storage;
using NuClear.Storage.API.ConnectionStrings;

using DataConnectionFactory = NuClear.Replication.Bulk.API.Factories.DataConnectionFactory;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    static class ReplicationMetdataProvider
    {
        public static readonly MetadataProvider Instance
            = new MetadataProvider(new IMetadataSource[] { new BulkReplicationMetadataSource() }, new IMetadataProcessor[0]);
    }

    public sealed class BulkReplicationAdapter<T> : ITestAction
        where T : IKey, new()
    {
        private readonly IConnectionStringSettings _connectionStringSettings;
        private readonly T _key;

        public BulkReplicationAdapter(ActMetadataElement metadata, IMetadataProvider metadataProvider, ConnectionStringSettingsAspect connectionStringSettings)
        {
            _key = new T();
            _connectionStringSettings = MappedConnectionStringSettings.CreateMappedSettings(
                connectionStringSettings,
                metadata,
                metadataProvider.GetMetadataSet<SchemaMetadataIdentity>().Metadata.Values.Cast<SchemaMetadataElement>().ToDictionary(x => x.Context, x => x));
        }

        public void Act()
        {
            var viewRemover = new ViewRemover(_connectionStringSettings);
            var connectionFactory = new DataConnectionFactory(_connectionStringSettings);
            var runner = new BulkReplicationRunner(ReplicationMetdataProvider.Instance, connectionFactory, viewRemover);

            runner.Run(_key.Key);
        }
    }
}