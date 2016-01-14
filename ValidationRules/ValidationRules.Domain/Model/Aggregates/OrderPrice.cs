namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// Связь заказа с прайс-листом, вычиляется на ознове позиций заказа из ERM
    /// </summary>
    public class OrderPrice
    {
        public long OrderId { get; set; }
        public long PriceId { get; set; }
    }
}