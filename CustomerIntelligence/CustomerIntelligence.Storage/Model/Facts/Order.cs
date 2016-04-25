using System;

namespace NuClear.CustomerIntelligence.Storage.Model.Facts
{
    public sealed class Order
    {
        public long Id { get; set; }

        public DateTimeOffset EndDistributionDateFact { get; set; }

        public long FirmId { get; set; }
    }
}