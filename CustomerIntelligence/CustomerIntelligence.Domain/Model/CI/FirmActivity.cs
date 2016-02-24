using System;

using NuClear.River.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.CI
{
    public sealed class FirmActivity : ICustomerIntelligenceAggregatePart, IAggregateValueObject
    {
        public long FirmId { get; set; }

        public DateTimeOffset? LastActivityOn { get; set; }
    }
}