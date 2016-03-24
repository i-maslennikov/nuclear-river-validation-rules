namespace NuClear.River.Common.Metadata.Model.Operations
{
    public sealed class RecalculateStatisticsOperation : IOperation
    {
        public RecalculateStatisticsOperation(StatisticsKey entityId)
        {
            EntityId = entityId;
        }

        public StatisticsKey EntityId { get; }

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

            return Equals((RecalculateStatisticsOperation)obj);
        }

        public override int GetHashCode()
        {
            return (EntityId.CategoryId.GetHashCode() * 397) ^ EntityId.ProjectId.GetHashCode();
        }

        private bool Equals(RecalculateStatisticsOperation other)
        {
            return EntityId.CategoryId == other.EntityId.CategoryId && EntityId.ProjectId == other.EntityId.ProjectId;
        }

        public override string ToString()
        {
            return string.Format("{0}(Project:{1}, Category:{2})", GetType().Name, EntityId.ProjectId, EntityId.CategoryId);
        }
    }
}
