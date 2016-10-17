using System;
using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.ThemeRules.Validation
{
    /// <summary>
    /// Для городов, в которых число одновременно активных тематик по-умолчанию не равно единице, должна выводиться ошибка
    /// 
    /// "Для подразделения {0} не указана тематика по умолчанию"
    /// Source: DefaultThemeMustBeSpecifiedValidationRule/DefaultThemeIsNotSpecified
    /// 
    /// "Для подразделения {0} установлено более одной тематики по умолчанию"
    /// Source: DefaultThemeMustBeSpecifiedValidationRule/MoreThanOneDefaultTheme
    /// </summary>
    public sealed class DefaultThemeMustBeExactlyOne : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.None)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public DefaultThemeMustBeExactlyOne(IQuery query) : base(query, MessageTypeCode.DefaultThemeMustBeExactlyOne)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var dates = query.For<Project.ProjectDefaultTheme>().Select(x => new { x.ProjectId, Date = x.Start })
                 .Union(query.For<Project.ProjectDefaultTheme>().Select(x => new {x.ProjectId, Date = x.End }))
                 .Union(query.For<Project.ProjectDefaultTheme>().Select(x => new { x.ProjectId, Date = DateTime.MinValue }));

            var projectPeriods = from date in dates
                                 let nextDate = dates.FirstOrDefault(x => x.ProjectId == date.ProjectId && x.Date > date.Date).Date
                                 select new
                                 {
                                   Start = date.Date,
                                   End = nextDate != null ? nextDate.Date : DateTime.MaxValue,
                                   date.ProjectId
                                 };

            var ruleResults = from projectPeriod in projectPeriods
                              let themeCount = query.For<Project.ProjectDefaultTheme>().Count(x => x.ProjectId == projectPeriod.ProjectId && x.Start < projectPeriod.End && x.End > projectPeriod.Start)
                              where themeCount != 1
                              select new Version.ValidationResult
                              {
                                  MessageParams = new XDocument(
                                          new XElement("root",
                                              new XElement("project",
                                                  new XAttribute("id", projectPeriod.ProjectId),
                                                  new XAttribute("name", query.For<Project>().Single(x => x.Id == projectPeriod.ProjectId).Name)),
                                              new XElement("message",
                                                  new XAttribute("themeCount", themeCount)))),
                                  PeriodStart = projectPeriod.Start,
                                  PeriodEnd = projectPeriod.End,
                                  ProjectId = projectPeriod.ProjectId,

                                  Result = RuleResult,
                              };

            return ruleResults;
        }
    }
}
