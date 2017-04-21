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
                from orderPeriod in query.For<Order.OrderPeriod>().Where(x => x.OrderId == orderPosition.OrderId)
                from period in query.For<Period>().Where(x => orderPeriod.Begin <= x.Start && x.End <= orderPeriod.End && x.ProjectId == orderPosition.ProjectId)
                select new { orderPosition.CategoryId, orderPosition.ProjectId, orderPeriod.OrderId, orderPeriod.Scope, period.Start, period.End };

            var oversales =
                from sale in sales
                let count = sales
                    .Count(x => x.CategoryId == sale.CategoryId
                                && x.ProjectId == sale.ProjectId
                                && x.Start == sale.Start
                                && Scope.CanSee(sale.Scope, x.Scope))
                where count > MaxPositionsPerCategory
                select new { sale.Start, sale.End, sale.ProjectId, sale.OrderId, sale.CategoryId, Count = count };

            var messages =
                from oversale in oversales.Distinct()
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object> { { "max", MaxPositionsPerCategory }, { "count", oversale.Count } },
                                    new Reference<EntityTypeCategory>(oversale.CategoryId),
                                    new Reference<EntityTypeProject>(oversale.ProjectId))
                                .ToXDocument(),

                        PeriodStart = oversale.Start,
                        PeriodEnd = oversale.End,
                        OrderId = oversale.OrderId,
                    };

            return messages;
        }
    }
}
