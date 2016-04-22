using System;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class Activity
    {
        public long Id { get; set; }

        public DateTimeOffset ModifiedOn { get; set; }

        public long? FirmId { get; set; }

        public long? ClientId { get; set; }
    }
}