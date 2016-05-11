using NuClear.CustomerIntelligence.StateInitialization.Host;
using NuClear.StateInitialization.Core.Commands;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    public interface IKey
    {
        ReplaceDataObjectsInBulkCommand Command { get; }
    }

    public sealed class Facts : IKey
    {
        public ReplaceDataObjectsInBulkCommand Command => BulkReplicationCommands.ErmToFacts;
    }

    public sealed class CustomerIntelligence : IKey
    {
        public ReplaceDataObjectsInBulkCommand Command => BulkReplicationCommands.FactsToCi;
    }
}