using System;

namespace NuClear.CustomerIntelligence.Domain.Commands
{
    public class RecalculateEntityCommand : IAggregateCommand
    {
        public RecalculateEntityCommand(Type aggregateRootType, long aggregateRootId, Type entityType, long entityId)
        {
            AggregateRootType = aggregateRootType;
            AggregateRootId = aggregateRootId;
            EntityType = entityType;
            EntityId = entityId;
        }

        public Type AggregateRootType { get; }
        public long AggregateRootId { get; }

        public Type EntityType { get; }
        public long EntityId { get; }
    }
}