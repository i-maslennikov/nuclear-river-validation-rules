namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    public sealed class OrderAssociatedPosition
    {
        public long OrderId { get; set; }
        public long CauseOrderPositionId { get; set; }
        public long CausePackagePositionId { get; set; }
        public long CauseItemPositionId { get; set; }

        public long PrincipalPositionId { get; set; }
        public long BindingType { get; set; }

        public bool HasNoBinding { get; set; }
        public long? Category3Id { get; set; }
        public long? FirmAddressId { get; set; }
        public long? Category1Id { get; set; }

        public string Source { get; set; }
    }
}