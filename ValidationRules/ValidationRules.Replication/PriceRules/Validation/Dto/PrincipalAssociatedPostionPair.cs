using System;

using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation.Dto
{
    public class PrincipalAssociatedPostionPair
    {
        public long FirmId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public long ProjectId { get; set; }
        public OrderPosition OrderPrincipalPosition { get; set; }
        public OrderAssociatedPosition OrderAssociatedPosition { get; set; }
        public NamesDto OrderPrincipalPositionNames { get; set; }
        public NamesDto OrderAssociatedPositionNames { get; set; }

        public class NamesDto
        {
            public string OrderNumber { get; set; }
            public string OrderPositionName { get; set; }
            public string ItemPositionName { get; set; }
        }
    }
}