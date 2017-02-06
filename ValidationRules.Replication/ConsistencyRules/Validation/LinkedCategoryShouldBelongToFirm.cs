using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, к которым привязана рубрика, не принадлежащая фирме должно выводиться предупреждение/ошибка, если объект привязки не является "рубрика множественная со звёздочкой".
    /// "В позиции {0} найдена рубрика {1}, не принадлежащая фирме заказа"
    /// 
    /// Source: LinkingObjectsOrderValidationRule
    /// </summary>
    public sealed class LinkedCategoryShouldBelongToFirm : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public LinkedCategoryShouldBelongToFirm(IQuery query) : base(query, MessageTypeCode.LinkedCategoryShouldBelongToFirm)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              from category in query.For<Order.InvalidCategory>().Where(x => x.OrderId == order.Id)
                              where category.State == InvalidCategoryState.NotBelongToFirm && !category.MayNotBelongToFirm
                              select new Version.ValidationResult
                                  {
                                      MessageParams = new XDocument(
                                          new XElement("root",
                                              new XElement("category",
                                                  new XAttribute("id", category.CategoryId),
                                                  new XAttribute("name", category.CategoryName)),
                                              new XElement("order",
                                                  new XAttribute("id", order.Id),
                                                  new XAttribute("name", order.Number)),
                                              new XElement("orderPosition",
                                                  new XAttribute("id", category.OrderPositionId),
                                                  new XAttribute("name", category.OrderPositionName)))),

                                      PeriodStart = order.BeginDistribution,
                                      PeriodEnd = order.EndDistributionPlan,
                                      OrderId = order.Id,

                                      Result = RuleResult,
                                  };

            return ruleResults;
        }
    }
}
