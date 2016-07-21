using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для проекта, в котором продано недостаточно рекламы в Position.Category должна выводиться ошибка.
    /// "Позиция {0} должна присутствовать в сборке в количестве от {1} до {2}. Фактическое количество позиций в месяц {6} - {3} (оформлено - {4}, содержит ошибки - {5})"
    /// В силу того, что мы (пока) не знаем, число заказов с ошибками, сократим сообщение (erm тоже так делает иногда)
    /// "Позиция {0} должна присутствовать в сборке в количестве от {1} до {2}. Фактическое количество позиций в месяц {3} - {4}"
    /// 
    /// Source: AdvertisementAmountOrderValidationRule/AdvertisementAmountErrorMessage
    /// todo: Нужно, чтобы ошибка не препятствовала одобрению заказа, но не давала собрать город. Можно ли использовать Scope?
    /// </summary>
    public sealed class MinimumAdvertisementAmountActor : IActor
    {
        public const int MessageTypeId = 16;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        private readonly ValidationRuleShared _validationRuleShared;

        public MinimumAdvertisementAmountActor(ValidationRuleShared validationRuleShared)
        {
            _validationRuleShared = validationRuleShared;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            return _validationRuleShared.ProcessRule(GetValidationResults, MessageTypeId);
        }

        private static IQueryable<Version.ValidationResult> GetValidationResults(IQuery query, long version)
        {
            var restrictionGrid = from restriction in query.For<AdvertisementAmountRestriction>()
                                  join pp in query.For<PricePeriod>() on restriction.PriceId equals pp.PriceId
                                  select new { Key = new { pp.Start, pp.OrganizationUnitId, restriction.CategoryCode }, restriction.Min, restriction.Max, restriction.CategoryName };

            var saleGrid = from position in query.For<AmountControlledPosition>()
                           join op in query.For<OrderPeriod>() on position.OrderId equals op.OrderId
                           group new { op.Start, op.OrganizationUnitId, position.CategoryCode }
                               by new { op.Start, op.OrganizationUnitId, position.CategoryCode } into groups
                           select new { groups.Key, Count = groups.Count() };

            var ruleViolations = from restriction in restrictionGrid
                                 from sale in saleGrid.Where(x => x.Key == restriction.Key).DefaultIfEmpty()
                                 where restriction.Min > (sale == null ? 0 : sale.Count)
                                 select new { restriction.Key, restriction.Min, restriction.Max, sale.Count, restriction.CategoryName };

            var projectRuleViolations = from period in query.For<Period>()
                                        join violation in ruleViolations on new { period.Start, period.OrganizationUnitId }
                                            equals new { violation.Key.Start, violation.Key.OrganizationUnitId }
                                        join project in query.For<Project>() on period.ProjectId equals project.Id
                                        select new Version.ValidationResult
                                            {
                                                MessageType = MessageTypeId,
                                                MessageParams =
                                                  new XDocument(new XElement("root",
                                                                new XElement("message",
                                                                            new XAttribute("min", violation.Min),
                                                                            new XAttribute("count", violation.Count),
                                                                            new XAttribute("name", violation.CategoryName),
                                                                            new XAttribute("month", period.Start)),
                                                                new XElement("project",
                                                                            new XAttribute("id", project.Id),
                                                                            new XAttribute("name", project.Name)))),

                                                PeriodStart = period.Start,
                                                PeriodEnd = period.End,
                                                ProjectId = period.ProjectId,
                                                VersionId = version,

                                                ReferenceType = EntityTypeIds.Project,
                                                ReferenceId = period.ProjectId,

                                                Result = RuleResult,
                                            };

            var orderRuleViolations = from position in query.For<AmountControlledPosition>()
                                      join op in query.For<OrderPeriod>() on position.OrderId equals op.OrderId
                                      join order in query.For<Order>() on op.OrderId equals order.Id
                                      join violation in ruleViolations on new { op.Start, op.OrganizationUnitId, position.CategoryCode }
                                          equals new { violation.Key.Start, violation.Key.OrganizationUnitId, violation.Key.CategoryCode }
                                      join period in query.For<Period>() on new { op.Start, op.OrganizationUnitId }
                                          equals new { period.Start, period.OrganizationUnitId }
                                      join project in query.For<Project>() on period.ProjectId equals project.Id
                                      select new Version.ValidationResult
                                          {
                                              MessageType = MessageTypeId,
                                              MessageParams =
                                                  new XDocument(new XElement("root",
                                                                new XElement("message",
                                                                            new XAttribute("min", violation.Min),
                                                                            new XAttribute("count", violation.Count),
                                                                            new XAttribute("name", violation.CategoryName),
                                                                            new XAttribute("month", period.Start)),
                                                                new XElement("order",
                                                                            new XAttribute("id", order.Id),
                                                                            new XAttribute("number", order.Number)),
                                                                new XElement("project",
                                                                            new XAttribute("id", project.Id),
                                                                            new XAttribute("number", project.Name)))),
                                              PeriodStart = period.Start,
                                              PeriodEnd = period.End,
                                              ProjectId = period.ProjectId,
                                              VersionId = version,

                                              ReferenceType = EntityTypeIds.Order,
                                              ReferenceId = position.OrderId,

                                              Result = RuleResult,
                                          };

            // В projectRuleViolations получаются сообщения с тегом проекта,
            // в orderRuleViolations получаются сообщения с тегом заказа.
            return orderRuleViolations.ToArray().Concat(projectRuleViolations.ToArray()).AsQueryable();
        }
    }
}
