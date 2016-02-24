using System.Linq;

using NuClear.DataTest.Metamodel;
using NuClear.DataTest.Metamodel.Dsl;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Processors.Concrete;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Replication.Bulk.API;
using NuClear.Replication.Bulk.API.Storage;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.ValidationRules.Domain;
using NuClear.ValidationRules.StateInitialization;


namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed class BulkReplicationAdapter<T> : ITestAction
        where T : IKey, new()
    {
        private static readonly MetadataProvider DefaultProvider
            = new MetadataProvider(
                new IMetadataSource[]
                    {
                        new BulkReplicationMetadataSource(),
                        new FactsReplicationMetadataSource(),
                        new ImportOrderValidationConfigMetadataSource(),
                        new AggregateConstructionMetadataSource()
                    },
                new IMetadataProcessor[] { new ReferencesEvaluatorProcessor() });

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
            var runner = new BulkReplicationRunner(DefaultProvider, connectionFactory, viewRemover);

            runner.Run(_key.Key);
        }
    }
}