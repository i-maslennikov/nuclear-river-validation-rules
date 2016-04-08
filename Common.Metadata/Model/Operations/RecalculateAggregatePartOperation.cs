namespace NuClear.River.Common.Metadata.Model.Operations
{
    public sealed class RecalculateAggregatePart : AggregateOperation
    {
        public RecalculateAggregatePart(EntityReference root, EntityReference entity)
            : base(root)
        {
            Entity = entity;
        }

        public EntityReference Entity { get; }

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

            return Equals((RecalculateAggregatePart)obj);
        }

        public override int GetHashCode()
        {
            var code = base.GetHashCode() * 397;
            code = (code * 397) ^ Entity.EntityKey.GetHashCode();
            code = (code * 397) ^ Entity.EntityType.Id;
            return code;
        }

        private bool Equals(RecalculateAggregatePart other)
        {
            return base.Equals(other)
                && Equals(Entity, other.Entity);
        }
    }
}