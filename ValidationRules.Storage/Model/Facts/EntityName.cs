namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class EntityName
    {
        public long Id { get; set; }
        public int TypeId { get; set; }

        public string Name { get; set; }

        public struct EntityNameKey
        {
            public long Id { get; set; }
            public int TypeId { get; set; }

            public bool Equals(EntityNameKey other)
            {
                return Id == other.Id && TypeId == other.TypeId;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is EntityNameKey && Equals((EntityNameKey)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Id.GetHashCode() * 397) ^ TypeId;
                }
            }
        }
    }
}
