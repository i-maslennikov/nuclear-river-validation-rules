using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Xml.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Actors.Validation
{
    /// <summary>
    /// Для проекта, в котором продано недостаточно рекламы в Position.Category должна выводиться ошибка.
    /// </summary>
    public sealed class MinimumAdvertisementAmountActor : IActor
    {
        private const int MessageTypeId = 1;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        private readonly IQuery _query;
        private readonly IBulkRepository<Version.ValidationResult> _repository;
        private readonly IBulkRepository<Version.ValidationResultForBulkDelete> _deleteRepository;

        public MinimumAdvertisementAmountActor(IQuery query, IBulkRepository<Version.ValidationResult> repository, IBulkRepository<Version.ValidationResultForBulkDelete> deleteRepository)
        {
            _query = query;
            _repository = repository;
            _deleteRepository = deleteRepository;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            // todo: привести в соответствие с созданием новой версии
            var currentVersion = _query.For<Version>().OrderByDescending(x => x.Id).FirstOrDefault()?.Id ?? 0;

            IReadOnlyCollection<Version.ValidationResult> sourceObjects;
            using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                // Запрос к данным источника посылаем вне транзакции, большой беды от этого быть не должно.
                sourceObjects = GetValidationResults(_query, currentVersion).ToArray();

                // todo: удалить, добавлено с целью отладки
                sourceObjects = sourceObjects.Where(x => x.PeriodStart >= DateTime.Parse("2016-06-01")).ToArray();

                scope.Complete();
            }

            // Данные в целевых таблицах меняем в одной большой транзакции (сейчас она управляется из хендлера)
            var forBulkDelete = new Version.ValidationResultForBulkDelete { MessageType = MessageTypeId, VersionId = currentVersion };
            _deleteRepository.Delete(new [] { forBulkDelete});
            _repository.Create(sourceObjects);

            return Array.Empty<IEvent>();
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
                                        select new Version.ValidationResult
                                            {
                                                MessageType = MessageTypeId,
                                                MessageParams =
                                                    new XDocument(new XElement("empty",
                                                                               new XAttribute("min", violation.Min),
                                                                               new XAttribute("count", violation.Count),
                                                                               new XAttribute("name", violation.CategoryName))),
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
                                      join violation in ruleViolations on new { op.Start, op.OrganizationUnitId, position.CategoryCode }
                                          equals new { violation.Key.Start, violation.Key.OrganizationUnitId, violation.Key.CategoryCode }
                                      join period in query.For<Period>() on new { op.Start, op.OrganizationUnitId }
                                          equals new { period.Start, period.OrganizationUnitId }
                                      select new Version.ValidationResult
                                          {
                                              MessageType = MessageTypeId,
                                              MessageParams =
                                                  new XDocument(new XElement("empty",
                                                                             new XAttribute("min", violation.Min),
                                                                             new XAttribute("count", violation.Count),
                                                                             new XAttribute("name", violation.CategoryName))),
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
            return projectRuleViolations.Union(orderRuleViolations);
        }
    }
}
