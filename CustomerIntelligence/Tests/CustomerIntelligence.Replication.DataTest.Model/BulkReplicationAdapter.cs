using NuClear.CustomerIntelligence.StateInitialization.EntryPoint;
using NuClear.DataTest.Metamodel.Dsl;
using NuClear.Replication.Bulk.API;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    public sealed class BulkReplicationAdapter<T> : ITestAction
        where T : IKey, new()
    {
        private readonly T _key;

        public BulkReplicationAdapter()
        {
            _key = new T();
        }

        public void Act()
        {
            var bulkReplicationActor = new BulkReplicationActor(new DataObjectTypesProviderFactory());
            bulkReplicationActor.ExecuteCommands(new[] { _key.Command });
        }
    }
}