using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для ЭРМ с указанными датами размещения, если общий период менее пяти дней, при единичной проверке должно выводиться предупреждение
    /// "Период не может быть менее пяти дней"
    /// 
    /// Source: CouponPeriodOrderValidationRule/AdvertisementPeriodError
    /// </summary>
    public sealed class OrderCouponPeriodMustNotBeLessFiveDays : ValidationResultAccessorBase
    {
        private const int MaxOffsetInDays = 5;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.None)
                                                                    .WhenMassRelease(Result.None);

        public OrderCouponPeriodMustNotBeLessFiveDays(IQuery query) : base(query, MessageTypeCode.OrderCouponPeriodMustNotBeLessFiveDays)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              from opa in query.For<Order.OrderPositionAdvertisement>().Where(x => x.OrderId == order.Id)
                              from advertisement in query.For<Advertisement>().Where(x => x.Id == opa.AdvertisementId)
                              from elementOffset in query.For<Advertisement.Coupon>().Where(x => x.AdvertisementId == advertisement.Id)
                              where elementOffset.DaysTotal < MaxOffsetInDays
                              select new Version.ValidationResult
                                  {
                                      MessageParams = new XDocument(new XElement("root",
                                          new XElement("order",
                                              new XAttribute("id", order.Id),
                                              new XAttribute("name", order.Number))
                                          )),

                                      PeriodStart = order.BeginDistributionDate,
                                      PeriodEnd = order.EndDistributionDatePlan,
                                      OrderId = order.Id,

                                      Result = RuleResult,
                                  };

            return ruleResults;
        }
    }
}
