using System;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для позиций (opa), у которых код номенклатуры "Баннер в рубрике Выгодные покупки с 2ГИС" а рубрика не "Выгодные покупки с 2ГИС", должна выводиться ошибка
    /// "Продан {0} в рубрику, не являющуюся рубрикой 'Выгодные покупки с 2ГИС'"
    /// 
    /// Source: IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule
    /// </summary>
    public sealed class AdvantageousPurchasesBannerMustBeSoldInTheSameCategory : ValidationResultAccessorBase
    {
        public AdvantageousPurchasesBannerMustBeSoldInTheSameCategory(IQuery query) : base(query, MessageTypeCode.AdvantageousPurchasesBannerMustBeSoldInTheSameCategory)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            // Позиции с кодом номенклатуры "Баннер в рубрике Выгодные покупки с 2ГИС" (296)
            // последний раз появились в прайс-листах в декабре 2013 года.
            // Нужно уточнить, но скорее всего, проверка будет не востребована.

            /*
                 select Prices.OrganizationUnitId, max(Prices.BeginDate)
                 from Billing.Positions
                      join Billing.PricePositions on PricePositions.PositionId = Positions.Id
                      join Billing.Prices on Prices.Id = PricePositions.PriceId
                 where CategoryCode = 296
                 group by Prices.OrganizationUnitId
                 order by max(Prices.BeginDate) desc
             */

            // PS. Получено добро на удаление этой проверки

            return Array.Empty<Version.ValidationResult>().AsQueryable();
        }
    }
}
