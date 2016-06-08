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

namespace NuClear.ValidationRules.Replication.Actors
{
    public sealed class AdvertisementAmountActor : IActor
    {
        // todo: даже эта проверка может быть разделена на две схожие:
        // проверка на минимальное и проверка на максимальное количество рекламы.
        // в чём разница? по первой менее критичная ошибка (warning в режиме единичной проверки в erm),
        // заказ с такой ошибкой может быть одобрен.
        // а вот заказ с позицией, превышающей лимит - одобрен быть не может.

        private const int MessageTypeId = 1;

        private readonly IQuery _query;
        private readonly IBulkRepository<Version.ValidationResult> _repository;
        private readonly IBulkRepository<Version.ValidationResultForBulkDelete> _deleteRepository;

        public AdvertisementAmountActor(IQuery query, IBulkRepository<Version.ValidationResult> repository, IBulkRepository<Version.ValidationResultForBulkDelete> deleteRepository)
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
                                  select new { Key = new { pp.Start, pp.ProjectId, restriction.CategoryCode }, restriction.Min, restriction.Max, restriction.CategoryName };

            var saleGrid = from position in query.For<AmountControlledPosition>()
                           join op in query.For<OrderPeriod>() on position.OrderId equals op.OrderId
                           group new { op.Start, op.ProjectId, position.CategoryCode }
                               by new { op.Start, op.ProjectId, position.CategoryCode } into groups
                           select new { groups.Key, Count = groups.Count() };

            var ruleViolations = from restriction in restrictionGrid
                                 from sale in saleGrid.Where(x => x.Key == restriction.Key).DefaultIfEmpty()
                                 where sale == null || restriction.Min > sale.Count || sale.Count > restriction.Max
                                 select new { restriction.Key, restriction.Min, restriction.Max, sale.Count, restriction.CategoryName };

            var ruleResults = from position in query.For<AmountControlledPosition>()
                              join op in query.For<OrderPeriod>() on position.OrderId equals op.OrderId
                              join violation in ruleViolations on new { op.Start, op.ProjectId, position.CategoryCode } equals
                                  new { violation.Key.Start, violation.Key.ProjectId, violation.Key.CategoryCode }
                              join period in query.For<Period>() on new { op.Start, op.ProjectId } equals new { period.Start, period.ProjectId }
                              select new Version.ValidationResult
                                  {
                                      MessageType = MessageTypeId,
                                  MessageParams =
                                          new XDocument(new XElement("empty",
                                                                     new XAttribute("min", violation.Min),
                                                                     new XAttribute("max", violation.Max),
                                                                     new XAttribute("count", violation.Count),
                                                                     new XAttribute("name", violation.CategoryName))),
                                  OrderId = position.OrderId,
                                  PeriodStart = period.Start,
                                  PeriodEnd = period.End,
                                  ProjectId = period.ProjectId,
                                  Result = 1,
                                  VersionId = version
                              };

            return ruleResults;
        }
    }
}
