using System;

namespace NuClear.CustomerIntelligence.Domain.Model.CI
{
    public sealed class FirmActivity
    {
        public long FirmId { get; set; }

        public DateTimeOffset? LastActivityOn { get; set; }
    }
}