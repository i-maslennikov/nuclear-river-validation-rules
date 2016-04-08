using NuClear.Model.Common.Entities;

namespace NuClear.River.Common.Metadata.Model.Operations
{
    public class EntityReference
    {
        public EntityReference(IEntityType entityType, object entityKey)
        {
            EntityType = entityType;
            EntityKey = entityKey;
        }

        public IEntityType EntityType { get; }
        public object EntityKey { get; }

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

            return Equals((EntityReference)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EntityType.GetHashCode() * 397) ^ EntityKey.GetHashCode();
            }
        }

        private bool Equals(EntityReference other)
        {
            return Equals(other.EntityKey, EntityKey) && Equals(other.EntityType, EntityType);
        }

        public override string ToString()
        {
            return $"{EntityType.Description}, {EntityKey}";
        }
    }
}