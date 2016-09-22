namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    /// <summary>
    /// Импортированная из ERM сущность заказа
    /// </summary>
    public sealed class Order
    {
        public long Id { get; set; }
        public long FirmId { get; set; }
        public string Number { get; set; }
    }
}