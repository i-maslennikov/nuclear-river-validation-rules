using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates;

namespace NuClear.ValidationRules.Replication.ThemeRules.Validation
{
    /// <summary>
    /// Для заказов, период размещения которых не вложен в период действия тематики, должна выводиться ошибка
    /// "Заказ {0} не может иметь продаж в тематику {1}, поскольку тематика действует не весь период размещения заказа"
    /// 
    /// Source: ThemePeriodOverlapsOrderPeriodValidationRule/ThemePeriodDoesNotOverlapOrderPeriod
    /// </summary>
    public sealed class ThemePeriodMustContainOrderPeriod : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public ThemePeriodMustContainOrderPeriod(IQuery query) : base(query, MessageTypeCode.ThemePeriodMustContainOrderPeriod)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              from orderTheme in query.For<Order.OrderTheme>().Where(x => x.OrderId == order.Id)
                              from theme in query.For<Theme>().Where(x => x.Id == orderTheme.ThemeId)
                              where theme.BeginDistribution > order.BeginDistributionDate ||  // тематика начинает размещаться позже заказа
                                    order.EndDistributionDateFact > theme.EndDistribution     // тематика оканчивает размещаться раньше заказа
                              select new Version.ValidationResult
                                  {
                                      MessageParams = new XDocument(new XElement("root",
                                                                                 new XElement("order",
                                                                                              new XAttribute("id", order.Id),
                                                                                              new XAttribute("number", order.Number)),
                                                                                 new XElement("theme",
                                                                                              new XAttribute("id", theme.Id),
                                                                                              new XAttribute("name", theme.Name))
                                                                     )),
                                      PeriodStart = order.BeginDistributionDate,
                                      PeriodEnd = order.EndDistributionDateFact,
                                      ProjectId = order.ProjectId,

                                      Result = RuleResult,
                                  };

            return ruleResults;
        }
    }
}
