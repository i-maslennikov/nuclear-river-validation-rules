using NuClear.StateInitialization.Core.Commands;
using NuClear.StateInitialization.Core.Storage;
using NuClear.ValidationRules.Storage;
using NuClear.ValidationRules.Storage.Identitites.Connections;

namespace NuClear.ValidationRules.StateInitialization.Host
{
    public static class BulkReplicationCommands
    {
        public static ReplicateInBulkCommand FactsToAggregates { get; } =
            new ReplicateInBulkCommand(
                new StorageDescriptor(FactsConnectionStringIdentity.Instance, Schema.Facts),
                new StorageDescriptor(AggregatesConnectionStringIdentity.Instance, Schema.Aggregates));

        public static ReplicateInBulkCommand ErmToFacts { get; } =
            new ReplicateInBulkCommand(
                new StorageDescriptor(ErmConnectionStringIdentity.Instance, Schema.Erm),
                new StorageDescriptor(FactsConnectionStringIdentity.Instance, Schema.Facts));
    }
}