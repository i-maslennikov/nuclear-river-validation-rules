using NuClear.River.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.CI
{
    public sealed class Territory : ICustomerIntelligenceAggregatePart, IAggregateEntity
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long ProjectId { get; set; }
    }
}