using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Эта часть проверки не должна вызываться для единичных заказов.
    /// 
    /// Для ЭРМ с указанными датами размещения, срок размещения не должн быть менее пяти дней
    /// 
    /// "Период размещения рекламного материала {0}, выбранного в позиции {1} должен захватывать 5 дней от текущего месяца размещения"
    /// 
    /// Source: CouponPeriodOrderValidationRule/AdvertisementPeriodEndsBeforeReleasePeriodBegins
    /// </summary>
    public sealed class OrderPeriodMustContainAdvertisementPeriodMass : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.None)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public OrderPeriodMustContainAdvertisementPeriodMass(IQuery query) : base(query, MessageTypeCode.OrderPeriodMustContainAdvertisementPeriodMass)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var dtos = from order in query.For<Order>()
                       from opa in query.For<Order.OrderPositionAdvertisement>().Where(x => x.OrderId == order.Id)
                       from offset in query.For<Advertisement.ElementOffsetInDays>().Where(x => x.AdvertisementId == opa.AdvertisementId)
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

            var periods = dtos.Where(x => x.BeginDistributionDate < x.BeginMonth).Select(x => new { x.OrderId, x.AdvertisementId, x.OrderPositionId, x.PositionId, Start = x.BeginDistributionDate, End = x.BeginMonth }).Union(
                          dtos.Where(x => x.EndMonth < x.EndDistributionDatePlan).Select(x => new { x.OrderId, x.AdvertisementId, x.OrderPositionId, x.PositionId, Start = x.EndMonth, End = x.EndDistributionDatePlan }));

            var ruleResults = from period in periods
                              from order in query.For<Order>().Where(x => x.Id == period.OrderId)
                              from advertisement in query.For<Advertisement>().Where(x => x.Id == period.AdvertisementId)
                              from position in query.For<Position>().Where(x => x.Id == period.PositionId)
                              select new Version.ValidationResult
                              {
                                MessageParams = new XDocument(new XElement("root",
                                    new XElement("order",
                                        new XAttribute("id", order.Id),
                                        new XAttribute("name", order.Number)),
                                    new XElement("orderPosition",
                                        new XAttribute("id", period.OrderPositionId),
                                        new XAttribute("name", position.Name)),
                                    new XElement("advertisement",
                                        new XAttribute("id", advertisement.Id),
                                        new XAttribute("name", advertisement.Name))
                                    )),

                                PeriodStart = period.Start,
                                PeriodEnd = period.End,
                                OrderId = order.Id,

                                Result = RuleResult,
                            };

            return ruleResults;
        }
    }
}
