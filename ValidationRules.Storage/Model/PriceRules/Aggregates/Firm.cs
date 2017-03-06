using System;

namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    public sealed class Firm
    {
        public long Id { get; set; }

        public class FirmPosition
        {
            public long FirmId { get; set; }
            public long OrderId { get; set; }
            public long OrderPositionId { get; set; }
            public long PackagePositionId { get; set; }
            public long ItemPositionId { get; set; }
            public bool HasNoBinding { get; set; }
            public long? Category1Id { get; set; }
            public long? Category3Id { get; set; }
            public long? FirmAddressId { get; set; }
            public long Scope { get; set; }
            public DateTime Begin { get; set; }
            public DateTime End { get; set; }
        }

        public class FirmAssociatedPosition
        {
            public long FirmId { get; set; }
            public long OrderId { get; set; }
            public long OrderPositionId { get; set; }
            public long PackagePositionId { get; set; }
            public long ItemPositionId { get; set; }
            public long PrincipalPositionId { get; set; }
            public int BindingType { get; set; }
            public PositionSources Source { get; set; }
        }
    }
}