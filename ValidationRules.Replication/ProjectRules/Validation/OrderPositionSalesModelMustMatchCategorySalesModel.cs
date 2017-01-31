using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.ProjectRules.Validation
{
    /// <summary>
    /// Для позиций заказов, с продажами в рубрики, не соответствующие модели продаж позиции, должна выводиться ошибка
    /// "Позиция "{0}" не может быть продана в рубрику "{1}" проекта "{2}""
    /// 
    /// Source: SalesModelRestrictionsOrderValidationRule
    /// </summary>
    public sealed class OrderPositionSalesModelMustMatchCategorySalesModel : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public OrderPositionSalesModelMustMatchCategorySalesModel(IQuery query) : base(query, MessageTypeCode.OrderPositionSalesModelMustMatchCategorySalesModel)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from restriction in query.For<Project.SalesModelRestriction>().Where(x => x.End > order.Begin && order.End > x.Begin && x.ProjectId == order.ProjectId)
                from adv in query.For<Order.CategoryAdvertisement>().Where(x => x.OrderId == order.Id && x.CategoryId == restriction.CategoryId && x.IsSalesModelRestrictionApplicable)
                where restriction.SalesModel != adv.SalesModel
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
                                    new XAttribute("id", order.Id)),
                                new XElement("project",
                                    new XAttribute("id", order.ProjectId)))),

                        PeriodStart = order.Begin > restriction.Begin ? order.Begin : restriction.Begin,
                        PeriodEnd = order.End < restriction.End ? order.End : restriction.End,
                        OrderId = order.Id,

                        Result = RuleResult,
                    };

            return ruleResults;
        }
    }
}