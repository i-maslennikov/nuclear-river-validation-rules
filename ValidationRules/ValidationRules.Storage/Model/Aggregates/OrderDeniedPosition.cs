namespace NuClear.ValidationRules.Storage.Model.Aggregates
{
    /// <summary>
    /// Представляет запрет, порождаемый одной из позиций заказа.
    /// Запрет распространяется не только на позиции заказа, к которому он привязан,
    /// но и ко всем заказа того-же периода той-же фирмы.
    /// </summary>
    public sealed class OrderDeniedPosition
    {
        public long OrderId { get; set; }
        public long ItemPositionId { get; set; }
        public long BindingType { get; set; }
        public long ExceptOrderPositionId { get; set; }

        public long? Category3Id { get; set; }
        public long? FirmAddressId { get; set; }
        public long? Category1Id { get; set; }
    }
}