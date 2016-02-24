using NuClear.River.Common.Metadata.Context;

namespace NuClear.River.Common.Metadata.Model.Operations
{
    public abstract class AggregateOperation : IOperation
    {
        protected AggregateOperation(Predicate context)
        {
            Context = context;
        }

        public Predicate Context { get; }

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
            return Context.GetHashCode();
        }

        private bool Equals(AggregateOperation other)
        {
            // Тип уже проверен
            return Context.Equals(other.Context);
        }
    }
}