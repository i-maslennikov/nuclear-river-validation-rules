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
        public ReplicateInBulkCommand Command => BulkReplicationCommands.ErmToFacts;
    }

    public sealed class Aggregates : IKey
    {
        public ReplicateInBulkCommand Command => BulkReplicationCommands.FactsToAggregates;
    }

    public sealed class Messages : IKey
    {
        public ReplicateInBulkCommand Command => BulkReplicationCommands.AggregatesToMessages;
    }
}