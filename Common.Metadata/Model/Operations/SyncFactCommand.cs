using System;

namespace NuClear.River.Common.Metadata.Model.Operations
{
    public sealed class SyncFactCommand : ICommand, IOperation
    {
        public SyncFactCommand(Type factType, long factId)
        {
            if (factType == null)
            {
                throw new ArgumentNullException("factType");
            }

            FactType = factType;
            FactId = factId;
        }

        public Type FactType { get; }

        public long FactId { get; }

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

            return Equals((SyncFactCommand)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = FactType?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ FactId.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{GetType().Name}<{FactType.Name}>({FactId})";
        }

        private bool Equals(SyncFactCommand other)
        {
            return FactType == other.FactType && FactId == other.FactId;
        }
    }
}