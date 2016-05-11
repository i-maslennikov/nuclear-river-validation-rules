using NuClear.StateInitialization.Core.Commands;
using NuClear.StateInitialization.Core.Storage;
using NuClear.ValidationRules.Storage;
using NuClear.ValidationRules.Storage.Identitites.Connections;

namespace NuClear.ValidationRules.StateInitialization.Host
{
    public static class BulkReplicationCommands
    {
        public static ReplaceDataObjectsInBulkCommand FactsToAggregates { get; } =
            new ReplaceDataObjectsInBulkCommand(
                new StorageDescriptor(FactsConnectionStringIdentity.Instance, Schema.Facts),
                new StorageDescriptor(AggregatesConnectionStringIdentity.Instance, Schema.Aggregates));

        public static ReplaceDataObjectsInBulkCommand ErmToFacts { get; } =
            new ReplaceDataObjectsInBulkCommand(
                new StorageDescriptor(ErmConnectionStringIdentity.Instance, Schema.Erm),
                new StorageDescriptor(FactsConnectionStringIdentity.Instance, Schema.Facts));
    }
}