using System;
using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.Specifications;
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
    public sealed class CouponMustBeSoldOnceAtTime : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public CouponMustBeSoldOnceAtTime(IQuery query) : base(query, MessageTypeCode.CouponMustBeSoldOnceAtTime)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var couponOverlapPeriods =
                from period in query.For<Order.CouponDistributionPeriod>()
                from intersectingPeriod in query.For<Order.CouponDistributionPeriod>()
                                                .Where(x => x.AdvertisementId == period.AdvertisementId && x.Begin < period.End && period.Begin < x.End && Scope.CanSee(period.Scope, x.Scope))
                where query.For<Order.CouponDistributionPeriod>()
                                                .Count(x => x.AdvertisementId == period.AdvertisementId && x.Begin < period.End && period.Begin < x.End && Scope.CanSee(period.Scope, x.Scope)) > 1
                let order = query.For<Order>().Single(x => x.Id == period.OrderId)
                let advertisement = query.For<Advertisement>().Single(x => x.Id == period.AdvertisementId)
                let position = query.For<Position>().Single(x => x.Id == intersectingPeriod.PositionId)
                select new
                    {
                        Key = new
                            {
                                order.ProjectId,

                                period.OrderId,
                                OrderNumber = order.Number,
                                period.AdvertisementId,
                                AdvertisementName = advertisement.Name,
                                period.Begin,
                                period.End,
                            },

                        Value = new
                            {
                                intersectingPeriod.PositionId,
                                intersectingPeriod.OrderPositionId,
                                OrderPositionName = position.Name,
                            },
                    };

            // Вычисления в памяти, поскольку linq2db сам в памяти группировки не умеет
            var data = couponOverlapPeriods.ToArray();
            var ruleResults = data.GroupBy(x => x.Key)
                                  .Select(coupon =>
                                      new Version.ValidationResult
                                          {
                                              MessageParams = new XDocument(
                                                  new XElement("root",
                                                      new XElement("advertisement",
                                                          new XAttribute("id", coupon.Key.AdvertisementId),
                                                          new XAttribute("name", coupon.Key.AdvertisementName)),
                                                      new XElement("order",
                                                          new XAttribute("id", coupon.Key.OrderId),
                                                          new XAttribute("number", coupon.Key.OrderNumber)),
                                                      coupon.Distinct().Select(x => new XElement("orderPosition",
                                                          new XAttribute("id", x.Value.OrderPositionId),
                                                          new XAttribute("name", x.Value.OrderPositionName))))),

                                              PeriodStart = coupon.Key.Begin,
                                              PeriodEnd = coupon.Key.End,
                                              OrderId = coupon.Key.OrderId,

                                              Result = RuleResult,
                                          });

            return ruleResults.AsQueryable();
        }
    }
}
