namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    /// <summary>
    /// Связь заказа с его позицией и позицией прайс-листа
    /// </summary>
    public sealed class OrderPricePosition
    {
        public long OrderId { get; set; }
        public long OrderPositionId { get; set; }
        public string OrderPositionName { get; set; }
        public long PriceId { get; set; }
        public bool IsActive{ get; set; }
    }
}