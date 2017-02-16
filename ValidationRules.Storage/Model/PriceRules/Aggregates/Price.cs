using System;

namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    /// <summary>
    /// Импортированная из ERM сущность прайс-листа
    /// </summary>
    public sealed class Price
    {
        public long Id { get; set; }
        public DateTime BeginDate { get; set; }

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

        /// <summary>
        /// Представляет превышение числа AssociatedPositionsGroup для PricePosition (поддерживается не больее одной)
        /// </summary>
        public sealed class AssociatedPositionGroupOvercount
        {
            public long PriceId { get; set; }
            public long PricePositionId { get; set; }
            public long PositionId { get; set; }
            public int Count { get; set; }
        }
    }
}