using System;
using System.Collections.Generic;

namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class Position
    {
        public const long CategoryCodeSelfAdvertisementOnlyOnPc = 287; // Самореклама только для ПК
        public const long CategoryCodeAdvertisementInCategory = 38; // Объявление в рубрике(Объявление под списком выдачи)

        public const long CategoryCodeBasicPackage = 303; // пакет "Базовый"
        public const long CategoryCodeContextBanner = 395122163464046280; // МКБ
        public const long CategoryCodePremiumPartnerAdvertising = 809065011136692321; // Реклама в профилях партнеров (приоритетное размещение)
        public const long CategoryCodeCallToAction = 809065011136692314; // Кнопка-действие
        public const long CategoryCodePartnerAdvertisingAddress = 809065011136692326; // Реклама в профилях партнеров (адреса)

        public const int BindingObjectTypeCategoryMultipleAsterix = 1;
        public const int BindingObjectTypeAddressMultiple = 35;

        public const int PositionsGroupMedia = 1;

        public const int ContentSalesWithoutContent = 1;
        public const int ContentSalesContentIsNotRequired = 2;

        public static readonly IReadOnlyCollection<long> CategoryCodesAllowNotLocatedOnTheMap = new long[]
            {
                11, // Рекламная ссылка
                14, // Выгодные покупки с 2ГИС
                26, // Комментарий к адресу
                809064675575595276, // Реклама в профилях партнеров (адрес)
            };

        /// <summary>
        /// Категории номенклатуры, для которых допускается несовпадение фирмы заказа и фирмы адреса привязки (продажи в чужие карточки)
        /// </summary>
        public static readonly IReadOnlyCollection<long> CategoryCodesAllowFirmMismatch = new[]
            {
                809065011136692320, // Реклама в профилях партнеров (партнеры)
                809065011136692321, // Реклама в профилях партнеров (приоритетное размещение)
                809065011136692326, // Реклама в профилях партнеров (адрес)
            };

        public static readonly IReadOnlyCollection<long> CategoryCodesPoiAddressCheck = new[]
            {
                448239782219049217, // Poi_Online_old
                809065011136692327  // Poi_Online
            };

        public long Id { get; set; }

        public int BindingObjectType { get; set; }
        public int SalesModel { get; set; }
        public int PositionsGroup { get; set; }

        public bool IsFirmMismatchAllowed { get; set; }

        public bool IsCompositionOptional { get; set; }
        public int ContentSales { get; set; }

        public bool IsControlledByAmount { get; set; }

        public long CategoryCode { get; set; }

        public bool IsDeleted { get; set; }
    }
}