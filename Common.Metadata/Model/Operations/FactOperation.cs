using System;

namespace NuClear.River.Common.Metadata.Model.Operations
{
    public sealed class FactOperation : IOperation
    {
        public FactOperation(Type factType, long factId)
        {
            if (factType == null)
            {
                throw new ArgumentNullException("factType");
            }

            FactType = factType;
            FactId = factId;
        }

        public Type FactType { get; private set; }

        public long FactId { get; private set; }

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
            return Equals((FactOperation)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (FactType != null ? FactType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ FactId.GetHashCode();
                return hashCode;
            }
        }

        private bool Equals(FactOperation other)
        {
            return FactType == other.FactType && FactId == other.FactId;
        }

        public override string ToString()
        {
            return string.Format("{0}<{1}>({2})", GetType().Name, FactType.Name, FactId);
        }
    }
}