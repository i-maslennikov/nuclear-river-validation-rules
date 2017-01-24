using System;

using NuClear.Replication.Core.Commands;

namespace NuClear.ValidationRules.Replication.Commands
{
    public abstract class AggregateCommand : IAggregateCommand
    {
        public Type AggregateRootType { get; }

        public long AggregateRootId { get; }

        protected AggregateCommand(Type aggregateRootType, long aggregateRootId)
        {
            AggregateRootType = aggregateRootType;
            AggregateRootId = aggregateRootId;
        }

        protected bool Equals(AggregateCommand other)
        {
            return AggregateRootType == other.AggregateRootType && AggregateRootId == other.AggregateRootId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((AggregateCommand)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (AggregateRootType.GetHashCode() * 397) ^ AggregateRootId.GetHashCode();
            }
        }

        public sealed class Initialize : AggregateCommand
        {
            public Initialize(Type aggregateRootType, long aggregateRootId) : base(aggregateRootType, aggregateRootId) { }
        }

        public sealed class Recalculate : AggregateCommand
        {
            public Recalculate(Type aggregateRootType, long aggregateRootId) : base(aggregateRootType, aggregateRootId) { }
        }

        public sealed class Destroy : AggregateCommand
        {
            public Destroy(Type aggregateRootType, long aggregateRootId) : base(aggregateRootType, aggregateRootId) { }
        }
    }
}