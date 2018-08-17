using NuClear.StateInitialization.Core.Commands;
using NuClear.StateInitialization.Core.Storage;
using NuClear.ValidationRules.Storage;
using NuClear.ValidationRules.Storage.Connections;

namespace NuClear.ValidationRules.StateInitialization.Host
{
    public static class BulkReplicationCommands
    {
        private static readonly ExecutionMode ParallelReplication = new ExecutionMode(4, false);

        public static ReplicateInBulkCommand AggregatesToMessages { get; } =
            ReplicateFromDbToDbCommand(
                new StorageDescriptor(AggregatesConnectionStringIdentity.Instance, Schema.Aggregates),
                new StorageDescriptor(MessagesConnectionStringIdentity.Instance, Schema.Messages));

        public static ReplicateInBulkCommand FactsToAggregates { get; } =
            ReplicateFromDbToDbCommand(
                new StorageDescriptor(FactsConnectionStringIdentity.Instance, Schema.Facts),
                new StorageDescriptor(AggregatesConnectionStringIdentity.Instance, Schema.Aggregates));

        public static ReplicateInBulkCommand ErmToFacts { get; } =
            ReplicateFromDbToDbCommand(
                new StorageDescriptor(ErmConnectionStringIdentity.Instance, Schema.Erm),
                new StorageDescriptor(FactsConnectionStringIdentity.Instance, Schema.Facts));

        public static ReplicateInBulkCommand AmsToFacts { get; } =
            new ReplicateInBulkCommand(new StorageDescriptor(AmsConnectionStringIdentity.Instance, null),
                                       new StorageDescriptor(FactsConnectionStringIdentity.Instance, Schema.Facts));

        public static ReplicateInBulkCommand RulesetsToFacts { get; } =
            new ReplicateInBulkCommand(new StorageDescriptor(RulesetConnectionStringIdentity.Instance, null),
                                       new StorageDescriptor(FactsConnectionStringIdentity.Instance, Schema.Facts),
                                       databaseManagementMode:DbManagementMode.UpdateTableStatistics);

        /// <summary>
        /// В databaseManagementMode исключен updatestatistics - причина, т.к. будет выполнен rebuild индексов, то
        /// статистика для индексов при этом будет автоматически пересчитана с FULLSCAN, нет смысла после этого делать updatestatistics
        /// с меньшим SampleRate потенциально ухудшая качество статистики
        /// </summary>
        private static ReplicateInBulkCommand ReplicateFromDbToDbCommand(StorageDescriptor from, StorageDescriptor to) =>
            new ReplicateInBulkCommand(from,
                                       to,
                                       executionMode: ParallelReplication,
                                       databaseManagementMode: DbManagementMode.DropAndRecreateConstraints | DbManagementMode.EnableIndexManagment);
    }
}