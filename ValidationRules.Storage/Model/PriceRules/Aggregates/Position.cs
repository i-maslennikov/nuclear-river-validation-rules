using System;

namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    /// <summary>
    /// Импортированная из ERM сущность номенклатурной позиции
    /// </summary>
    public sealed class Position
    {
        public const int AdvertisementInCategory = 38; // Объявление в рубрике(Объявление под списком выдачи)

        public long Id { get; set; }
        public long CategoryCode { get; set; }
    }
}