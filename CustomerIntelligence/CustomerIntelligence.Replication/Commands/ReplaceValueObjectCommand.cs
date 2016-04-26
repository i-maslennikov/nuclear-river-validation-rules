using NuClear.Replication.Core.Commands;

namespace NuClear.CustomerIntelligence.Replication.Commands
{
    public class ReplaceValueObjectCommand : IReplaceValueObjectCommand
    {
        public ReplaceValueObjectCommand(long aggregateRootId, long? entityId = null)
        {
            AggregateRootId = aggregateRootId;
            EntityId = entityId;
        }

        public long AggregateRootId { get; }
        public long? EntityId { get; }
    }
}