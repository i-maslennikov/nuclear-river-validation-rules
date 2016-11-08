using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.ProjectRules.Validation
{
    /// <summary>
    /// Для проектов, где есть продажи в рубрики без указанного ограничения стоимости клика, должна выводиться ошибка.
    /// "Для рубрики {0} в проекте {1} не указан минимальный CPC"
    /// 
    /// Source: IsCostPerClickRestrictionMissingOrderValidationRule
    /// </summary>
    public sealed class ProjectMustContainCostPerClickMinimumRestriction : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public ProjectMustContainCostPerClickMinimumRestriction(IQuery query) : base(query, MessageTypeCode.ProjectMustContainCostPerClickMinimumRestriction)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from project in query.For<Project>()
                from order in query.For<Order>().Where(x => x.ProjectId == project.Id)
                from position in query.For<Order.CostPerClickAdvertisement>().Where(x => x.OrderId == order.Id)
                from category in query.For<Category>().Where(x => x.Id == position.CategoryId)
                let restrictionExist = query.For<Project.CostPerClickRestriction>().Any(x => x.ProjectId == order.ProjectId && x.CategoryId == position.CategoryId)
                where !restrictionExist
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
                        OrderId = order.Id,

                        Result = RuleResult,
                    };

            return ruleResults;
        }
    }
}