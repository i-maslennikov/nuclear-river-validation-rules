using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates;

namespace NuClear.ValidationRules.Replication.ThemeRules.Validation
{
    /// <summary>
    /// Для тематик, которые имеют продажи и привязаны к неактивным рубрикам, должна выводиться ошибка
    /// "Тематика {0} использует удаленную рубрику {1}"
    /// 
    /// Source: ThemeCategoriesValidationRule/ThemeUsesInactiveCategory
    /// </summary>
    public sealed class ThemeCategoryShouldBeValid : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.None)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public ThemeCategoryShouldBeValid(IQuery query) : base(query, MessageTypeCode.ThemeCategoryShouldBeValid)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var orderProjects = query.For<Order>().Select(x => new { x.Id, x.BeginDistributionDate, x.EndDistributionDateFact, ProjectId = x.SourceProjectId })
                         .Union(query.For<Order>().Select(x => new { x.Id, x.BeginDistributionDate, x.EndDistributionDateFact, ProjectId = x.DestProjectId }));

            var invalidPeriods = (from order in orderProjects
                                  from orderTheme in query.For<Order.OrderTheme>().Where(x => x.OrderId == order.Id)
                                  from invalidCategory in query.For<Theme.InvalidCategory>().Where(x => x.ThemeId == orderTheme.ThemeId)
                                  select new
                                  {
                                     orderTheme.ThemeId,
                                     invalidCategory.CategoryId,
                                     order.BeginDistributionDate,
                                     order.EndDistributionDateFact,
                                     order.ProjectId
                                  }).Distinct();

            var ruleResults = from invalidPeriod in invalidPeriods
                              select new Version.ValidationResult
                                  {
                                      MessageParams = new XDocument(new XElement("root",
                                                                                 new XElement("theme",
                                                                                              new XAttribute("id", invalidPeriod.ThemeId),
                                                                                              new XAttribute("name", query.For<Theme>().Single(x => x.Id == invalidPeriod.ThemeId).Name)),
                                                                                 new XElement("category",
                                                                                              new XAttribute("id", invalidPeriod.CategoryId),
                                                                                              new XAttribute("name", query.For<Category>().Single(x => x.Id == invalidPeriod.CategoryId).Name))
                                                                     )),
                                      PeriodStart = invalidPeriod.BeginDistributionDate,
                                      PeriodEnd = invalidPeriod.EndDistributionDateFact,
                                      ProjectId = invalidPeriod.ProjectId,

                                      Result = RuleResult,
                                  };

            return ruleResults;
        }
    }
}
