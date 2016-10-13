using System;
using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для фирм (но выводим в заказы), размещающих один РМ в более чем одной позиции "выгодные покупки", должна выводиться ошибка
    /// "Купон на скидку {0} прикреплён к нескольким позициям: {1}"
    /// 
    /// Source: CouponIsUniqueForFirmOrderValidationRule/CouponIsBoundToMultiplePositionTemplate
    /// </summary>
    public sealed class CouponMustBeUniqueForFirm : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public CouponMustBeUniqueForFirm(IQuery query) : base(query, MessageTypeCode.CouponMustBeUniqueForFirm)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var dates = query.For<Order>().Select(x => new { Date = x.BeginDistributionDate, x.FirmId})
                              .Union(query.For<Order>().Select(x => new { Date = x.EndDistributionDatePlan, x.FirmId}));
            dates = dates.Select(x => new { x.Date, x.FirmId });

            var periods =
                from date in dates
                let nextDate = dates.Where(x => x.FirmId == date.FirmId && x.Date > date.Date).Min(x => (DateTime?)x.Date)
                where nextDate.HasValue
                select new { date.FirmId, Start = date.Date, End = nextDate.Value };

            var couponDtos = from period in periods
                            from order in query.For<Order>().Where(x => x.FirmId == period.FirmId && period.Start >= x.BeginDistributionDate && period.End <= x.EndDistributionDatePlan)
                            from coupon in query.For<Order.CouponOrderPosition>().Where(x => x.OrderId == order.Id)
                            select new
                            {
                                period.FirmId,
                                coupon.AdvertisementId,

                                period.Start,
                                period.End,
                            };
            var couponDoubles = from couponDto in couponDtos
                                group couponDto by couponDto
                                into grps
                                where grps.Count() > 1
                                select grps.Key;
            // TODO: можно вынести couponDoubles в отдельный value object

            var ruleResults = from order in query.For<Order>()
                              from coupon in query.For<Order.CouponOrderPosition>().Where(x => x.OrderId == order.Id)
                              from couponDouble in couponDoubles.Where(x => x.AdvertisementId == coupon.AdvertisementId &&
                                                                            x.Start >= order.BeginDistributionDate && x.End <= order.EndDistributionDatePlan)
                               select new Version.ValidationResult
                               {
                                   MessageParams = new XDocument(new XElement("root",
                                                                                     new XElement("order",
                                                                                                 new XAttribute("id", order.Id),
                                                                                                 new XAttribute("number", order.Number)),
                                                                                     new XElement("orderPosition",
                                                                                                 new XAttribute("id", coupon.OrderPositionId),
                                                                                                 new XAttribute("name", query.For<Position>().Single(x => x.Id == coupon.PositionId).Name)),
                                                                                     new XElement("advertisement",
                                                                                                 new XAttribute("id", coupon.AdvertisementId),
                                                                                                 new XAttribute("name", query.For<Advertisement>().Single(x => x.Id == coupon.AdvertisementId).Name))
                                                                                     )),
                                   PeriodStart = couponDouble.Start,
                                   PeriodEnd = couponDouble.End,
                                   ProjectId = order.ProjectId,

                                   Result = RuleResult,
                               };

            return ruleResults;
        }
    }
}
