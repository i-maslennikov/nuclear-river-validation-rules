namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    /// <summary>
    /// Связь прайс-листа с номеклатурной позицией, импортируется из ERM
    /// </summary>
    public sealed class AdvertisementAmountRestriction
    {
        public long PriceId { get; set; }
        public long CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public bool MissingMinimalRestriction { get; set; }
    }
}