using System;
using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.ProjectRules.Validation
{
    /// <summary>
    /// Для заказов, с продажами в рубрики для которых не указана стоимость клика, должна выводиться ошибка
    /// "Для позиции {0} в рубрику {1} отсутствует CPC"
    /// 
    /// Source: IsCostPerClickMissingOrderValidationRule
    /// </summary>
    public sealed class OrderPositionCostPerClickMustBeSpecified : ValidationResultAccessorBase
    {
        public const int CostPerClickSalesModel = 12; // erm: MultiPlannedProvision

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public OrderPositionCostPerClickMustBeSpecified(IQuery query) : base(query, MessageTypeCode.OrderPositionCostPerClickMustBeSpecified)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from adv in query.For<Order.CategoryAdvertisement>().Where(x => x.SalesModel == CostPerClickSalesModel && x.OrderId == order.Id)
                where !query.For<Order.CostPerClickAdvertisement>().Any(x => x.OrderPositionId == adv.OrderPositionId && x.CategoryId == adv.CategoryId)
                select new Version.ValidationResult
                    {
                        MessageParams = new XDocument(
                            new XElement("root",
                                new XElement("category",
                                    new XAttribute("id", adv.CategoryId)),
                                new XElement("opa",
                                    new XElement("orderPosition", new XAttribute("id", adv.OrderPositionId)),
                                    new XElement("position", new XAttribute("id", adv.PositionId))),
                                new XElement("order",
                                    new XAttribute("id", order.Id)))),

                        PeriodStart = order.Begin,
                        PeriodEnd = order.End,
                        OrderId = order.Id,

                        Result = RuleResult,
                    };

            return ruleResults;
        }
    }
}