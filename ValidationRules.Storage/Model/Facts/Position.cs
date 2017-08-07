using System.Collections.Generic;

namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class Position
    {
        public const long CategoryCodeAdvantageousPurchaseWith2Gis = 14; // Выгодные покупки с 2ГИС (купоны с периодом размещения)
        public const long CategoryCodeSelfAdvertisementOnlyOnPc = 287; // Самореклама только для ПК
        public const long CategoryCodeAdvertisementInCategory = 38; // Объявление в рубрике(Объявление под списком выдачи)

        public const int BindingObjectTypeCategoryMultipleAsterix = 1;

        public const int PlatformIndependent = 0;
        public const int PlatformDesktop = 1;

        public const int PositionsGroupMedia = 1;

        public static readonly IReadOnlyCollection<long> CategoryCodesAllowNotLocatedOnTheMap = new long[]
            {
                11, // Рекламная ссылка
                14, // Выгодные покупки с 2ГИС
                26, // Комментарий к адресу
            };

        public long Id { get; set; }

        public int BindingObjectType { get; set; }
        public int SalesModel { get; set; }
        public int PositionsGroup { get; set; }


        public bool IsCompositionOptional { get; set; }
        public bool IsContentSales { get; set; }
        public bool IsControlledByAmount { get; set; }

        public long CategoryCode { get; set; }
        public int Platform { get; set; }

        public bool IsDeleted { get; set; }
    }
}