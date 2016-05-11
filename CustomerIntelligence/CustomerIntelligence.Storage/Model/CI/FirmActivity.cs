using System;

namespace NuClear.CustomerIntelligence.Storage.Model.CI
{
    public sealed class FirmActivity
    {
        public long FirmId { get; set; }

        public DateTimeOffset? LastActivityOn { get; set; }
    }
}