using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.Replication.Core;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для заказов и проектов, с превышением числа проданных тематик, должна выводиться ошибка.
    /// "Слишком много продаж в тематику {0}. Продано {1} позиций вместо {2} возможных"
    /// 
    /// Source: ThemePositionCountValidationRule
    /// </summary>
    public sealed class AdvertisementCountPerThemeShouldBeLimitedActor
    {
        public const int MessageTypeId = 16;

        private const int MaxPositionsPerTheme = 10;

        private readonly ValidationRuleShared _validationRuleShared;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public AdvertisementCountPerThemeShouldBeLimitedActor(ValidationRuleShared validationRuleShared)
        {
            _validationRuleShared = validationRuleShared;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            return _validationRuleShared.ProcessRule(GetValidationResults, MessageTypeId);
        }

        private static IQueryable<Version.ValidationResult> GetValidationResults(IQuery query, long version)
        {
            var data = query.For<OrderPosition>().Where(x => x.ThemeId != null)
                            .SelectMany(position => query.For<OrderPeriod>().Where(x => x.OrderId == position.OrderId && x.Scope == 0), (position, period) => new { position, period })
                            .GroupBy(x => new { x.period.Start, x.period.OrganizationUnitId, x.position.ThemeId })
                            .Select(x => new { x.Key.Start, x.Key.OrganizationUnitId, x.Key.ThemeId, Count = x.Count() });

            var messages = from order in query.For<Order>()
                           from orderPosition in query.For<OrderPosition>().Where(x => x.OrderId == order.Id && x.ThemeId != null)
                           from orderPeriod in query.For<OrderPeriod>().Where(x => x.OrderId == order.Id)
                           from period in query.For<Period>().Where(x => x.Start == orderPeriod.Start && x.OrganizationUnitId == orderPeriod.OrganizationUnitId)
                           from record in data.Where(x => x.OrganizationUnitId == orderPeriod.OrganizationUnitId && x.Start == orderPeriod.Start && x.ThemeId == orderPosition.ThemeId)
                           from theme in query.For<Theme>().Where(x => x.Id == orderPosition.ThemeId)
                           let count = orderPeriod.Scope == 0 ? record.Count : record.Count + 1
                           where count > MaxPositionsPerTheme
                           select new Version.ValidationResult
                               {
                                   MessageType = MessageTypeId,
                                   MessageParams =
                                       new XDocument(new XElement("root",
                                                                  new XElement("message",
                                                                               new XAttribute("max", MaxPositionsPerTheme),
                                                                               new XAttribute("count", count),
                                                                               new XAttribute("name", theme.Name),
                                                                               new XAttribute("month", orderPeriod.Start)),
                                                                  new XElement("order",
                                                                               new XAttribute("id", order.Id),
                                                                               new XAttribute("number", order.Number)))),
                                   PeriodStart = period.Start,
                                   PeriodEnd = period.End,
                                   ProjectId = period.ProjectId,
                                   VersionId = version,

                                   Result = RuleResult,
                               };

            return messages;
        }
    }
}
