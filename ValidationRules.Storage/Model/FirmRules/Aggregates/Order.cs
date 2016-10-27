using System;

namespace NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates
{
    public sealed class Order
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public long FirmId { get; set; }
        public string Number { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public long Scope { get; set; }

        public sealed class FirmOrganiationUnitMismatch
        {
            public long OrderId { get; set; }
        }

        public sealed class NotApplicapleForDesktopPosition
        {
            public long OrderId { get; set; }
        }

        public sealed class SelfAdvertisementPosition
        {
            public long OrderId { get; set; }
        }

        public class CategoryPurchase
        {
            public long OrderId { get; set; }
            public long CategoryId { get; set; }
        }
    }
}