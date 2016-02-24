using NuClear.River.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// Связь прайс-листа с номеклатурной позицией, импортируется из ERM
    /// </summary>
    public sealed class AdvertisementAmountRestriction : IAggregateValueObject
    {
        public long PriceId { get; set; }
        public long PositionId { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
    }
}