namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// Связь заказа с номенклатурной позицией, импортируется из ERM.
    /// </summary>
    public class OrderPosition
    {
        public long OrderId { get; set; }
        public long PositionId { get; set; }
    }
}