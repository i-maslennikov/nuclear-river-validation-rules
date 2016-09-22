using NuClear.StateInitialization.Core.Commands;
using NuClear.StateInitialization.Core.Storage;
using NuClear.ValidationRules.Storage;
using NuClear.ValidationRules.Storage.Identitites.Connections;

namespace NuClear.ValidationRules.StateInitialization.Host
{
    public static class BulkReplicationCommands
    {
        public static ReplicateInBulkCommand AggregatesToMessages { get; } =
            new ReplicateInBulkCommand(
                new StorageDescriptor(AggregatesConnectionStringIdentity.Instance, Schema.Aggregates),
                new StorageDescriptor(MessagesConnectionStringIdentity.Instance, Schema.Messages));

        public static ReplicateInBulkCommand FactsToAggregates { get; } =
            new ReplicateInBulkCommand(
                new StorageDescriptor(FactsConnectionStringIdentity.Instance, Schema.Facts),
                new StorageDescriptor(AggregatesConnectionStringIdentity.Instance, Schema.Aggregates));

        public static ReplicateInBulkCommand ErmToFacts { get; } =
            new ReplicateInBulkCommand(
                new StorageDescriptor(ErmConnectionStringIdentity.Instance, Schema.Erm),
                new StorageDescriptor(FactsConnectionStringIdentity.Instance, Schema.Facts));

        public static ReplicateInBulkCommand AggregatesToMessagesTest { get; } =
            new ReplicateInBulkCommand(
                new StorageDescriptor(AggregatesConnectionStringIdentity.Instance, Schema.Aggregates),
                new StorageDescriptor(MessagesConnectionStringIdentity.Instance, Schema.Messages),
                DbManagementMode.None);

        public static ReplicateInBulkCommand FactsToAggregatesTest { get; } =
            new ReplicateInBulkCommand(
                new StorageDescriptor(FactsConnectionStringIdentity.Instance, Schema.Facts),
                new StorageDescriptor(AggregatesConnectionStringIdentity.Instance, Schema.Aggregates),
                DbManagementMode.None);

        public static ReplicateInBulkCommand ErmToFactsTest { get; } =
            new ReplicateInBulkCommand(
                new StorageDescriptor(ErmConnectionStringIdentity.Instance, Schema.Erm),
                new StorageDescriptor(FactsConnectionStringIdentity.Instance, Schema.Facts),
                DbManagementMode.None);
    }
}