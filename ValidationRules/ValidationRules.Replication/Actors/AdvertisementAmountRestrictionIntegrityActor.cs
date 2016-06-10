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
    /// <summary>
    /// Для прайс-листов, в которых для позиций с контроллируемым количеством не указан минимум должна выводиться ошибка.
    /// </summary>
    public sealed class AdvertisementAmountRestrictionIntegrityActor : IActor
    {
        private const int MessageTypeId = 2;

        private readonly IQuery _query;
        private readonly IBulkRepository<Version.ValidationResult> _repository;
        private readonly IBulkRepository<Version.ValidationResultForBulkDelete> _deleteRepository;

        public AdvertisementAmountRestrictionIntegrityActor(IQuery query, IBulkRepository<Version.ValidationResult> repository, IBulkRepository<Version.ValidationResultForBulkDelete> deleteRepository)
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
            _deleteRepository.Delete(new[] { forBulkDelete });
            _repository.Create(sourceObjects);

            return Array.Empty<IEvent>();
        }

        private static IQueryable<Version.ValidationResult> GetValidationResults(IQuery query, long version)
        {
            var ruleResults = from restriction in query.For<AdvertisementAmountRestriction>().Where(x => x.MissingMinimalRestriction)
                              join pp in query.For<PricePeriod>() on restriction.PriceId equals pp.PriceId
                              join period in query.For<Period>() on new { pp.Start, pp.ProjectId } equals new { period.Start, period.ProjectId }
                              join op in query.For<OrderPeriod>() on new { pp.Start, pp.ProjectId } equals new { op.Start, op.ProjectId }
                              select new Version.ValidationResult
                                  {
                                      MessageType = MessageTypeId,
                                      MessageParams = new XDocument(new XElement("empty", new XAttribute("name", restriction.CategoryName))),
                                      PeriodStart = period.Start,
                                      PeriodEnd = period.End,
                                      ProjectId = pp.ProjectId,
                                      VersionId = version,

                                      ReferenceType = EntityTypeIds.Project,
                                      ReferenceId = pp.ProjectId,
                              };

            return ruleResults;
        }
    }
}
