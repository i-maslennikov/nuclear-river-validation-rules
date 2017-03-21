using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для заказов, у которых есть неразмещающиеся купоны, должна выводиться ошибка
    /// "Период размещения рекламного материала {0}, выбранного в позиции {1} должен захватывать 5 дней от текущего месяца размещения"
    /// 
    /// Source: CouponPeriodOrderValidationRule/AdvertisementPeriodEndsBeforeReleasePeriodBegins
    /// </summary>
    public sealed class OrderCouponPeriodMustBeInRelease : ValidationResultAccessorBase
    {
        public OrderCouponPeriodMustBeInRelease(IQuery query) : base(query, MessageTypeCode.OrderCouponPeriodMustBeInRelease)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var dtos =
                from order in query.For<Order>()
                from opa in query.For<Order.OrderPositionAdvertisement>().Where(x => x.OrderId == order.Id)
                from offset in query.For<Advertisement.Coupon>().Where(x => x.AdvertisementId == opa.AdvertisementId)
                select new
                    {
                        opa.OrderId,
                        opa.AdvertisementId,
                        opa.OrderPositionId,
                        opa.PositionId,
                        order.BeginDistributionDate,
                        order.EndDistributionDatePlan,
                        offset.BeginMonth,
                        offset.EndMonth
                    };

            var leftPeriods =
                dtos.Where(x => x.BeginDistributionDate < x.BeginMonth)
                    .Select(x => new { x.OrderId, x.AdvertisementId, x.OrderPositionId, x.PositionId, Start = x.BeginDistributionDate, End = x.BeginMonth });

            var rightPeriods =
                dtos.Where(x => x.EndMonth < x.EndDistributionDatePlan)
                    .Select(x => new { x.OrderId, x.AdvertisementId, x.OrderPositionId, x.PositionId, Start = x.EndMonth, End = x.EndDistributionDatePlan });

            var ruleResults =
                from period in leftPeriods.Union(rightPeriods)
                from order in query.For<Order>().Where(x => x.Id == period.OrderId)
                from advertisement in query.For<Advertisement>().Where(x => x.Id == period.AdvertisementId)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrder>(order.Id),
                                    new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                        new Reference<EntityTypeOrderPosition>(period.OrderPositionId),
                                        new Reference<EntityTypePosition>(period.PositionId)),
                                    new Reference<EntityTypeAdvertisement>(advertisement.Id))
                                .ToXDocument(),

                        PeriodStart = period.Start,
                        PeriodEnd = period.End,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}
