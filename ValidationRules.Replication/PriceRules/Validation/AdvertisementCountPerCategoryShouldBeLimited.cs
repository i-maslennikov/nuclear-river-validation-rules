using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для проектов, с превышением продаж определённой номенклатуры в рубрику, должна выводиться ошибка в массовой и предупреждение в единичной.
    /// "В рубрику {0} заказано слишком много объявлений: Заказано {1}, допустимо не более {2}"
    /// 
    /// Source: AdvertisementForCategoryAmountOrderValidationRule
    /// 
    /// Внимание, в этой проверке как наследство erm есть две совершенно различные вещи, обозначаемые словом Category: рубрика и категория номенклатуры.
    /// 
    /// Q: Если "чистая" продажа в рубрику одна, а продаж в рубрику адреса - много, проверка должна срабатывать?
    /// A: Эти объявления (CategoryCode = 38) привязываются только к рубрике (или фирме, если пакет) - поэтому этот вопрос не важен.
    /// </summary>
    public sealed class AdvertisementCountPerCategoryShouldBeLimited : ValidationResultAccessorBase
    {
        private const int MaxPositionsPerCategory = 2;

        public AdvertisementCountPerCategoryShouldBeLimited(IQuery query) : base(query, MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var sales =
                from orderPosition in query.For<Order.OrderCategoryPosition>()
                from orderPeriod in query.For<Period.OrderPeriod>().Where(x => x.OrderId == orderPosition.OrderId)
                select new { orderPosition.OrderId, orderPeriod.Scope, orderPeriod.Start, orderPeriod.OrganizationUnitId, orderPosition.CategoryId };

            var saleCounts =
                sales.GroupBy(x => new { x.Scope, x.Start, x.OrganizationUnitId, x.CategoryId })
                     .Select(x => new { x.Key, Count = x.Count() });

            var oversales =
                from sale in sales
                let count = saleCounts.Where(x => x.Key.CategoryId == sale.CategoryId
                                                  && x.Key.OrganizationUnitId == sale.OrganizationUnitId
                                                  && x.Key.Start == sale.Start
                                                  && Scope.CanSee(sale.Scope, x.Key.Scope))
                                      .Sum(x => x.Count)
                where count > MaxPositionsPerCategory
                select new { sale.Start, sale.OrganizationUnitId, sale.OrderId, sale.CategoryId, Count = count };

            var messages =
                from oversale in oversales.Distinct()
                join period in query.For<Period>() on new { oversale.Start, oversale.OrganizationUnitId } equals new { period.Start, period.OrganizationUnitId }
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object> { { "max", MaxPositionsPerCategory }, { "count", oversale.Count } },
                                    new Reference<EntityTypeCategory>(oversale.CategoryId),
                                    new Reference<EntityTypeProject>(period.ProjectId))
                                .ToXDocument(),

                        PeriodStart = period.Start,
                        PeriodEnd = period.End,
                        OrderId = oversale.OrderId,
                    };

            return messages;
        }
    }
}
