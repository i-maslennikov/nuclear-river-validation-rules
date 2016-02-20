using System;

namespace NuClear.River.Common.Metadata.Model.Operations
{
    public abstract class AggregateOperation : IOperation
    {
        protected AggregateOperation(Type aggregateType, long aggregateId)
        {
            if (aggregateType == null)
            {
                throw new ArgumentNullException("aggregateType");
            }

            AggregateType = aggregateType;
            AggregateId = aggregateId;
        }

        public Type AggregateType { get; private set; }

        public long AggregateId { get; private set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((AggregateOperation)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (AggregateType != null ? AggregateType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ AggregateId.GetHashCode();
                return hashCode;
            }
        }

        private bool Equals(AggregateOperation other)
        {
            return AggregateType == other.AggregateType && AggregateId == other.AggregateId;
        }

        public override string ToString()
        {
            return string.Format("{0}<{1}>({2})", GetType().Name, AggregateType.Name, AggregateId);
        }
    }
}