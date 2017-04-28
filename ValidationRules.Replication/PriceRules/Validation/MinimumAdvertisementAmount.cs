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
    /// Для проекта, в котором продано недостаточно рекламы в Position.Category должна выводиться ошибка.
    /// "Позиция {0} должна присутствовать в сборке в количестве от {1} до {2}. Фактическое количество позиций в месяц {6} - {3} (оформлено - {4}, содержит ошибки - {5})"
    /// В силу того, что мы (пока) не знаем, число заказов с ошибками, сократим сообщение (erm тоже так делает иногда)
    /// "Позиция {0} должна присутствовать в сборке в количестве от {1} до {2}. Фактическое количество позиций в месяц {3} - {4}"
    /// 
    /// Source: AdvertisementAmountOrderValidationRule/AdvertisementAmountErrorMessage
    /// </summary>
    public sealed class MinimumAdvertisementAmount : ValidationResultAccessorBase
    {
        public MinimumAdvertisementAmount(IQuery query) : base(query, MessageTypeCode.MinimumAdvertisementAmount)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var restrictionGrid =
                from restriction in query.For<Price.AdvertisementAmountRestriction>()
                from pp in query.For<Price.PricePeriod>().Where(x => x.PriceId == restriction.PriceId)
                select new { pp.Begin, pp.End, pp.ProjectId, restriction.CategoryCode, restriction.Min, restriction.Max, restriction.CategoryName };

            var saleGrid =
                from position in query.For<Order.AmountControlledPosition>()
                from orderPeriod in query.For<Order.OrderPeriod>().Where(x => x.OrderId == position.OrderId)
                from period in query.For<Period>().Where(x => orderPeriod.Begin <= x.Start && x.End <= orderPeriod.End)
                select new { position.OrderId, period.Start, period.End, orderPeriod.Scope, position.ProjectId, position.CategoryCode };

            var violations =
                from sale in saleGrid
                from restriction in restrictionGrid.Where(x => x.Begin <= sale.Start && sale.End <= x.End && x.ProjectId == sale.ProjectId && x.CategoryCode == sale.CategoryCode)
                let count = saleGrid.Count(x => x.CategoryCode == sale.CategoryCode &&
                                                x.ProjectId == sale.ProjectId &&
                                                x.Start == sale.Start &&
                                                Scope.CanSee(sale.Scope, x.Scope))
                select new { sale.OrderId, sale.Start, sale.End, restriction.Min, restriction.Max, Count = count, restriction.CategoryName };

            var messages =
                from violation in violations
                where violation.Count < violation.Min
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object>
                                        {
                                            { "min", violation.Min },
                                            { "max", violation.Max },
                                            { "count", violation.Count },
                                            { "name", violation.CategoryName },
                                            { "month", violation.Start },
                                        },
                                    new Reference<EntityTypeOrder>(violation.OrderId))
                                .ToXDocument(),

                        PeriodStart = violation.Start,
                        PeriodEnd = violation.End,
                        OrderId = violation.OrderId,
                    };

            return messages;
        }
    }
}
