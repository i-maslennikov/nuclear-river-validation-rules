using NuClear.River.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.CI
{
    public sealed class Client : ICustomerIntelligenceAggregatePart, IAggregateRoot
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long CategoryGroupId { get; set; }
    }
}