using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates;

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
        public DefaultThemeMustBeExactlyOne(IQuery query) : base(query, MessageTypeCode.DefaultThemeMustBeExactlyOne)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var dates =
                query.For<Project.ProjectDefaultTheme>().Select(x => new { x.ProjectId, Date = x.Start })
                     .Union(query.For<Project.ProjectDefaultTheme>().Select(x => new { x.ProjectId, Date = x.End }))
                     .Union(query.For<Project>().Select(x => new { ProjectId = x.Id, Date = System.DateTime.MinValue })); // Фиктивное начало для каждого проекта, даже если в нём нет ни одной тематики по умолчанию

            var projectPeriods =
                from date in dates
                from nextDate in dates.Where(x => x.ProjectId == date.ProjectId && x.Date > date.Date).OrderBy(x => x.Date).Take(1).DefaultIfEmpty()
                select new
                    {
                        Start = date.Date,
                        End = nextDate != null ? nextDate.Date : System.DateTime.MaxValue,
                        date.ProjectId
                    };

            var ruleResults =
                from projectPeriod in projectPeriods
                let themeCount = query.For<Project.ProjectDefaultTheme>().Count(x => x.ProjectId == projectPeriod.ProjectId && x.Start < projectPeriod.End && x.End > projectPeriod.Start)
                where themeCount != 1
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object> { { "themeCount", themeCount } },
                                    new Reference<EntityTypeProject>(projectPeriod.ProjectId))
                                .ToXDocument(),

                        PeriodStart = projectPeriod.Start,
                        PeriodEnd = projectPeriod.End,
                        ProjectId = projectPeriod.ProjectId,
                    };

            return ruleResults;
        }
    }
}
