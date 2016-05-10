using System;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.Actors;
using NuClear.CustomerIntelligence.StateInitialization.Host;
using NuClear.DataTest.Metamodel;
using NuClear.DataTest.Metamodel.Dsl;
using NuClear.Metamodeling.Provider;
using NuClear.StateInitialization.Core;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    public sealed class BulkReplicationAdapter<T> : ITestAction
        where T : IKey, new()
    {
        private readonly T _key;
        private readonly IConnectionStringSettings _connectionStringSettings;
        private readonly Type _anchor = typeof(AggregateActor);

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
            var bulkReplicationActor = new BulkReplicationActor(new DataObjectTypesProviderFactory(), _connectionStringSettings);
            bulkReplicationActor.ExecuteCommands(new[] { _key.Command });
        }
    }
}