using System.Collections.Generic;

namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class Position
    {
        public const long CategoryCodeAdvantageousPurchaseWith2Gis = 14; // Выгодные покупки с 2ГИС (купоны с периодом размещения)
        public const long CategoryCodeSelfAdvertisementOnlyOnPc = 287; // Самореклама только для ПК
        public const long CategoryCodeAdvertisementInCategory = 38; // Объявление в рубрике(Объявление под списком выдачи)

        public const int BindingObjectTypeCategoryMultipleAsterix = 1;
        public const int BindingObjectTypeAddressMultiple = 35;

        public const int PlatformIndependent = 0;
        public const int PlatformDesktop = 1;

        public const int PositionsGroupMedia = 1;

        public static readonly IReadOnlyCollection<long> CategoryCodesAllowNotLocatedOnTheMap = new long[]
            {
                11, // Рекламная ссылка
                14, // Выгодные покупки с 2ГИС
                26, // Комментарий к адресу
            };

        /// <summary>
        /// Категории номенклатуры, для которых допускается несовпадение фирмы заказа и фирмы адреса привязки (продажи в чужие карточки)
        /// </summary>
        public static readonly IReadOnlyCollection<long> CategoryCodesAllowFirmMismatch = new long[]
            {
                809065011136692320, // Реклама в профилях партнеров (партнеры)
                809065011136692321, // Реклама в профилях партнеров (приоритетное размещение)
                809065011136692326, // Реклама в профилях партнеров (адрес)
            };

        public long Id { get; set; }

        public long? AdvertisementTemplateId { get; set; }
        public int BindingObjectType { get; set; }
        public int SalesModel { get; set; }
        public int PositionsGroup { get; set; }

        public bool IsFirmMismatchAllowed { get; set; }

        public bool IsCompositionOptional { get; set; }
        public bool IsControlledByAmount { get; set; }
        public bool IsComposite { get; set; }

        public long CategoryCode { get; set; }
        public int Platform { get; set; }

        public bool IsDeleted { get; set; }
    }
}