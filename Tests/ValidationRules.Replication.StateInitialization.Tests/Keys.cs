using NuClear.StateInitialization.Core.Commands;
using NuClear.ValidationRules.StateInitialization.Host;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public interface IKey
    {
        ReplicateInBulkCommand Command { get; }
    }

    public sealed class Facts : IKey
    {
        public ReplicateInBulkCommand Command => BulkReplicationCommands.ErmToFactsTest;
    }

    public sealed class Aggregates : IKey
    {
        public ReplicateInBulkCommand Command => BulkReplicationCommands.FactsToAggregatesTest;
    }

    public sealed class Messages : IKey
    {
        public ReplicateInBulkCommand Command => BulkReplicationCommands.AggregatesToMessagesTest;
    }
}