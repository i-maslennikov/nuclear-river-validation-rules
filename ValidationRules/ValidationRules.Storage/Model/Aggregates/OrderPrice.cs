namespace NuClear.ValidationRules.Storage.Model.Aggregates
{
    /// <summary>
    /// Связь заказа с прайс-листом, вычиляется на ознове позиций заказа из ERM
    /// </summary>
    public sealed class OrderPrice
    {
        public long OrderId { get; set; }
        public long PriceId { get; set; }
    }
}