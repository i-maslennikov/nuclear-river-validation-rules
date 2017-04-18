using System;

namespace NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates
{
    public sealed class Order
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public bool IsDraft { get; set; }

        public sealed class AddressAdvertisementNonOnTheMap
        {
            public long OrderId { get; set; }
            public long OrderPositionId { get; set; }
            public long PositionId { get; set; }
            public long AddressId { get; set; }
        }

        public class CategoryAdvertisement
        {
            public const int CostPerClickSalesModel = 12; // erm: MultiPlannedProvision

            public long OrderId { get; set; }
            public long OrderPositionId { get; set; }
            public long PositionId { get; set; }
            public long CategoryId { get; set; }
            public int SalesModel { get; set; }
            public bool IsSalesModelRestrictionApplicable { get; set; }
        }

        public class CostPerClickAdvertisement
        {
            public long OrderId { get; set; }
            public long OrderPositionId { get; set; }
            public long PositionId { get; set; }
            public long CategoryId { get; set; }
            public decimal Bid { get; set; }
        }
    }
}