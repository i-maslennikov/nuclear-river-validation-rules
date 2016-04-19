using System;

namespace NuClear.River.Common.Metadata.Model.Operations
{
    public abstract class AggregateOperation : IOperation
    {
        protected AggregateOperation(EntityReference aggregateRoot)
        {
            if (aggregateRoot == null)
            {
                throw new ArgumentNullException(nameof(aggregateRoot));
            }

            AggregateRoot = aggregateRoot;
        }

        public EntityReference AggregateRoot { get; }


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
                return (AggregateRoot.GetHashCode() * 397) ^ this.GetType().GetHashCode();
            }
        }

        private bool Equals(AggregateOperation other)
        {
            return Equals(this.AggregateRoot, other.AggregateRoot);
        }

        public override string ToString()
        {
            return $"{GetType().Name}({AggregateRoot})";
        }
    }
}