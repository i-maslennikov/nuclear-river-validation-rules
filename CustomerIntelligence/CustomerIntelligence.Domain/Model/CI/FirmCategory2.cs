using NuClear.River.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.CI
{
    public sealed class FirmCategory2 : ICustomerIntelligenceAggregatePart, IAggregateValueObject
    {
        public long FirmId { get; set; }

        public long CategoryId { get; set; }
    }
}