namespace NuClear.CustomerIntelligence.Replication
{
    public sealed class StatisticsKey
    {
        public long ProjectId { get; set; }
        public long CategoryId { get; set; }

        private bool Equals(StatisticsKey other)
        {
            return ProjectId == other.ProjectId && CategoryId == other.CategoryId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is StatisticsKey && Equals((StatisticsKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ProjectId.GetHashCode() * 397) ^ CategoryId.GetHashCode();
            }
        }
    }
}