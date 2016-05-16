using NuClear.StateInitialization.Core.Commands;
using NuClear.ValidationRules.StateInitialization.Host;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public interface IKey
    {
        ReplaceDataObjectsInBulkCommand Command { get; }
    }

    public sealed class Facts : IKey
    {
        public ReplaceDataObjectsInBulkCommand Command => BulkReplicationCommands.ErmToFacts;
    }

    public sealed class Aggregates : IKey
    {
        public ReplaceDataObjectsInBulkCommand Command => BulkReplicationCommands.FactsToAggregates;
    }
}