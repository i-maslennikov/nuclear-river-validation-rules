using NuClear.River.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.CI
{
    public sealed class ProjectCategory : ICustomerIntelligenceAggregatePart, IAggregateValueObject
    {
        public long ProjectId { get; set; }

        public long CategoryId { get; set; }

        public string Name { get; set; }

        public int Level { get; set; }

        public long? ParentId { get; set; }

        public int SalesModel { get; set; }
    }
}