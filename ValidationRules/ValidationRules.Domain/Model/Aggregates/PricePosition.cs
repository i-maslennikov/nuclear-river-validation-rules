namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// Связь прайс-листа с номеклатурной позицией, импортируется из ERM
    /// </summary>
    public class PricePosition
    {
        public long PriceId { get; set; }
        public long PositionId { get; set; }
    }
}