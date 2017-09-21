using System.Collections.Generic;

using NuClear.StateInitialization.Core.Commands;
using NuClear.StateInitialization.Core.Storage;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.ValidationRules.OperationsProcessing.AmsFactsFlow;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage;
using NuClear.ValidationRules.Storage.Identitites.Connections;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.StateInitialization.Host
{
    public static class BulkReplicationCommands
    {
        private static readonly ExecutionMode ParallelReplication = new ExecutionMode(4, false);

        public static ReplicateInBulkCommand AggregatesToMessages { get; } =
            new ReplicateInBulkCommand(
                new StorageDescriptor(AggregatesConnectionStringIdentity.Instance, Schema.Aggregates),
                new StorageDescriptor(MessagesConnectionStringIdentity.Instance, Schema.Messages),
                executionMode: ParallelReplication);

        public static ReplicateInBulkCommand FactsToAggregates { get; } =
            new ReplicateInBulkCommand(
                new StorageDescriptor(FactsConnectionStringIdentity.Instance, Schema.Facts),
                new StorageDescriptor(AggregatesConnectionStringIdentity.Instance, Schema.Aggregates),
                executionMode: ParallelReplication);

        public static ReplicateInBulkCommand ErmToFacts { get; } =
            new ReplicateInBulkCommand(
                new StorageDescriptor(ErmConnectionStringIdentity.Instance, Schema.Erm),
                new StorageDescriptor(FactsConnectionStringIdentity.Instance, Schema.Facts),
                executionMode: ParallelReplication);

        public static IReadOnlyList<MemoryReplicateInBulkCommand> AmsToFacts(IConnectionStringSettings connectionStringSettings)
        {
            var amsStateInit = new AmsStateInit(connectionStringSettings);
            var replicationCommands = new[] { new ReplaceDataObjectCommand(typeof(Advertisement), amsStateInit.GeReplicationDtos(AmsFactsFlow.Instance)) };

            return new[]
            {
                new MemoryReplicateInBulkCommand(replicationCommands,
                    new StorageDescriptor(FactsConnectionStringIdentity.Instance, Schema.Facts)),

                new MemoryReplicateInBulkCommand(replicationCommands,
                    new StorageDescriptor(FactsConnectionStringIdentity.Instance, Schema.Facts),
                    DbManagementMode.All & ~DbManagementMode.TruncateTable)
            };
        }

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