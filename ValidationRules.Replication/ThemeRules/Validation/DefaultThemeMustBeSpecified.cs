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
    public sealed class DefaultThemeMustBeSpecified : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.None)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public DefaultThemeMustBeSpecified(IQuery query) : base(query, MessageTypeCode.DefaultThemeMustBeSpecified)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var themePeriodStarts = query.For<Theme>().Select(x => new { x.Id, Date = x.BeginDistribution, ThemeCount = 1 })
                                    .Union(query.For<Theme>().Select(x => new { x.Id, Date = x.EndDistribution, ThemeCount = 0 }))
                                    .Union(query.For<Theme>().Select(x => new { x.Id, Date = DateTime.MinValue, ThemeCount = 0 }));
            themePeriodStarts = themePeriodStarts.Select(x => new { x.Id, x.Date, x.ThemeCount });

            var themePeriodEnds = query.For<Theme>().Select(x => new { x.Id, Date = x.BeginDistribution, ThemeCount = 0 })
                                       .Union(query.For<Theme>().Select(x => new { x.Id, Date = x.EndDistribution, ThemeCount = 1 }))
                                       .Union(query.For<Theme>().Select(x => new { x.Id, Date = DateTime.MaxValue, ThemeCount = 0 }));
            themePeriodEnds = themePeriodEnds.Select(x => new { x.Id, x.Date, x.ThemeCount });

            var endAfterBegins = from themePeriodStart in themePeriodStarts
                              from themePeriodEnd in themePeriodEnds.Where(x => x.Date > themePeriodStart.Date)
                              select new
                              {
                                  ThemeStartId = themePeriodStart.Id,
                                  Start = themePeriodStart.Date,
                                  StartThemeCount = themePeriodStart.ThemeCount,

                                  ThemeEndId = themePeriodEnd.Id,
                                  End = themePeriodEnd.Date,
                                  EndThemeCount = themePeriodEnd.ThemeCount,
                              };

            var themeCounts = from endAfterBegin in endAfterBegins
                                       from endAfterBegin2 in endAfterBegins.Where(x => x.ThemeStartId == endAfterBegin.ThemeStartId &&
                                                                                        x.Start == endAfterBegin.Start &&
                                                                                        x.End < endAfterBegin.End).DefaultIfEmpty()
                                       where endAfterBegin2 == null
                                       select new
                                       {
                                           endAfterBegin.ThemeStartId,
                                           endAfterBegin.Start,
                                           endAfterBegin.End,
                                           ThemeCount = endAfterBegin.ThemeStartId == endAfterBegin.ThemeEndId ? endAfterBegin.StartThemeCount : endAfterBegin.StartThemeCount + endAfterBegin.EndThemeCount,
                                       };

            var ruleResults = from project in query.For<Project>()
                              from projectThemes in query.For<Project.ProjectTheme>().Where(x => x.ProjectId == project.Id).DefaultIfEmpty()
                              from theme in query.For<Theme>().Where(x => x.Id == projectThemes.ThemeId && x.IsDefault).DefaultIfEmpty() // тематика по умолчанию
                              from themeCount in themeCounts.Where(x => x.ThemeStartId == theme.Id && x.ThemeCount != 1).DefaultIfEmpty() // тематики либо нет либо несколько
                              select new Version.ValidationResult
                              {
                                  MessageParams = new XDocument(new XElement("root",
                                                                                 new XElement("project",
                                                                                              new XAttribute("id", project.Id),
                                                                                              new XAttribute("name", project.Name)),
                                                                                 new XElement("message",
                                                                                              new XAttribute("themeCount", themeCount.ThemeCount))
                                                                     )),
                                  PeriodStart = themeCount == null ? DateTime.MinValue : themeCount.Start,
                                  PeriodEnd = themeCount == null ? DateTime.MaxValue : themeCount.End,
                                  ProjectId = project.Id,

                                  Result = RuleResult,
                              };

            return ruleResults;
        }
    }
}
