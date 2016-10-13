﻿using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для ЭРМ с указанными датами размещения, срок размещения не должн быть менее пяти дней
    /// 
    /// "Период размещения рекламного материала {0}, выбранного в позиции {1} должен захватывать 5 дней от текущего месяца размещения"
    /// 
    /// Source: CouponPeriodOrderValidationRule/AdvertisementPeriodEndsBeforeReleasePeriodBegins
    /// </summary>
    public sealed class OrderPeriodMustContainAdvertisementPeriod : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public OrderPeriodMustContainAdvertisementPeriod(IQuery query) : base(query, MessageTypeCode.OrderPeriodMustContainAdvertisementPeriod)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {

            var ruleResults = from order in query.For<Order>()
                              from fail in query.For<Order.AdvertisementPeriodNotInOrderPeriod>().Where(x => x.OrderId == order.Id)
                              select new Version.ValidationResult
                              {
                                  MessageParams = new XDocument(new XElement("root",
                                                                                      new XElement("order",
                                                                                                  new XAttribute("id", order.Id),
                                                                                                  new XAttribute("number", order.Number)),
                                                                                      new XElement("orderPosition",
                                                                                                  new XAttribute("id", fail.OrderPositionId),
                                                                                                  new XAttribute("name", query.For<Position>().Single(x => x.Id == fail.PositionId).Name)),
                                                                                      new XElement("advertisement",
                                                                                                  new XAttribute("id", fail.AdvertisementId),
                                                                                                  new XAttribute("name", query.For<Advertisement>().Single(x => x.Id == fail.AdvertisementId).Name))
                                                                                      )),
                                  PeriodStart = order.BeginDistributionDate,
                                  PeriodEnd = order.EndDistributionDatePlan,
                                  ProjectId = order.ProjectId,

                                  Result = RuleResult,
                              };

            return ruleResults;
        }
    }
}
