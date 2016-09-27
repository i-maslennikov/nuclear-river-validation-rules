using System;
using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.ProjectRules.Validation
{
    /// <summary>
    /// Для заказов, в позиции которого выставлена ставка меньше минимально допустимой для этого города/рубрики, должна выводиться ошибка.
    /// "Для позиции {0} в рубрику {1} указан CPC меньше минимального"
    /// 
    /// Source: CostPerClickOrderValidationRule
    /// </summary>
    public sealed class OrderPositionCostPerClickMustNotBeLessMinimum : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public OrderPositionCostPerClickMustNotBeLessMinimum(IQuery query) : base(query, MessageTypeCode.OrderPositionCostPerClickMustNotBeLessMinimum)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from bid in query.For<Order.CostPerClickBid>().Where(x => x.OrderId == order.Id)
                from restriction in query.For<Project.MinimumCostPerClick>().Where(x => x.ProjectId == order.ProjectId && x.CategoryId == bid.CategoryId)
                from category in query.For<Category>().Where(x => x.Id == bid.CategoryId)
                from project in query.For<Project>().Where(x => x.Id == order.ProjectId)
                select new Version.ValidationResult
                    {
                        MessageParams = new XDocument(
                            new XElement("root",
                                new XElement("category",
                                    new XAttribute("id", category.Id),
                                    new XAttribute("name", category.Name)),
                                new XElement("project",
                                    new XAttribute("id", project.Id),
                                    new XAttribute("name", project.Name)),
                                new XElement("order",
                                    new XAttribute("id", order.Id),
                                    new XAttribute("number", order.Number)))),
                        PeriodStart = order.Begin,
                        PeriodEnd = order.End,
                        ProjectId = order.ProjectId,
                        Result = RuleResult,
                    };

            return ruleResults;
        }
    }
}