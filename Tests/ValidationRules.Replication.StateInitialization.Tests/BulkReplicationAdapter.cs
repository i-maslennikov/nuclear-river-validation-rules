using NuClear.DataTest.Metamodel.Dsl;
using NuClear.StateInitialization.Core.Actors;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.ValidationRules.StateInitialization.Host;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed class BulkReplicationAdapter<T> : ITestAction
        where T : IKey, new()
    {
        private readonly T _key;
        private readonly IConnectionStringSettings _connectionStringSettings;

        public BulkReplicationAdapter()
        {
            _key = new T();
            _connectionStringSettings = new RunnerConnectionStringSettings();
        }

        public void Act()
        {
            var bulkReplicationActor = new BulkReplicationActor(new DataObjectTypesProviderFactory(), _connectionStringSettings);
            bulkReplicationActor.ExecuteCommands(new[] { _key.Command });
        }
    }
}