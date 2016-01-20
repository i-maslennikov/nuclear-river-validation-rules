namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// Связь заказа с номенклатурной позицией, импортируется из ERM.OrderPosition + ERM.OrderPositionAdv
    /// </summary>
    public class OrderPosition
    {
        public long OrderId { get; set; }
        public long? PackagePositionId { get; set; }
        public long ItemPositionId { get; set; }
        public long? CategoryId { get; set; }
        public long? FirmAddressId { get; set; }
    }
}