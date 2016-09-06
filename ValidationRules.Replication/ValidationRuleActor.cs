using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.AccountRules.Validation;
using NuClear.ValidationRules.Replication.ConsistencyRules.Validation;
using NuClear.ValidationRules.Replication.PriceRules.Validation;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication
{
    // todo: Интересно, что будет, если сохранять данные в базу только после прогона всех проверок?
    public sealed class ValidationRuleActor : IActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<Version.ValidationResult> _repository;
        private readonly IBulkRepository<Version.ValidationResultForBulkDelete> _deleteRepository;
        private readonly ValidationRuleRegistry _registry;

        public ValidationRuleActor(IQuery query, IBulkRepository<Version.ValidationResult> repository, IBulkRepository<Version.ValidationResultForBulkDelete> deleteRepository, IStorageBasedDataObjectAccessor<Version.ValidationResult> accessor)
        {
            _query = query;
            _repository = repository;
            _deleteRepository = deleteRepository;
            _registry = new ValidationRuleRegistry(query);
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var currentVersion = _query.For<Version>().OrderByDescending(x => x.Id).FirstOrDefault()?.Id ?? 0;
            return _registry.CreateAccessors().SelectMany(accessor => ProcessRule(accessor, currentVersion)).ToArray();
        }

        public IReadOnlyCollection<IEvent> ProcessRule(IValidationResultAccessor accessor, long currentVersion)
        {
            IReadOnlyCollection<Version.ValidationResult> sourceObjects;
            using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                // Запрос к данным источника посылаем вне транзакции, большой беды от этого быть не должно.
                sourceObjects = accessor.GetSource().ApplyVersion(currentVersion).ToArray();

                // todo: удалить, добавлено с целью отладки
                sourceObjects = sourceObjects.Where(x => x.PeriodStart >= DateTime.Parse("2016-06-01")).ToArray();

                scope.Complete();
            }

            // Данные в целевых таблицах меняем в одной большой транзакции (сейчас она управляется из хендлера)
            var forBulkDelete = new Version.ValidationResultForBulkDelete { MessageType = accessor.MessageTypeId, VersionId = currentVersion };
            _deleteRepository.Delete(new[] { forBulkDelete });
            _repository.Create(sourceObjects);

            return Array.Empty<IEvent>();
        }

        private sealed class ValidationRuleRegistry
        {
            private readonly IQuery _query;

            public ValidationRuleRegistry(IQuery query)
            {
                _query = query;
            }

            public IEnumerable<IValidationResultAccessor> CreateAccessors()
            {
                yield return new BargainScanShouldPresent(_query);
                yield return new BillsPeriodShouldMatchOrder(_query);
                yield return new LegalPersonProfileBargainShouldNotBeExpired(_query);
                yield return new LegalPersonProfileWarrantyShouldNotBeExpired(_query);
                yield return new LegalPersonShouldHaveAtLeastOneProfile(_query);
                yield return new LinkedCategoryAsterixMayBelongToFirm(_query);
                yield return new LinkedCategoryFirmAddressShouldBeValid(_query);
                yield return new LinkedCategoryShouldBeActive(_query);
                yield return new LinkedCategoryShouldBelongToFirm(_query);
                yield return new LinkedFirmAddressShouldBeValid(_query);
                yield return new LinkedFirmShouldBeValid(_query);
                yield return new OrderBeginDistrubutionShouldBeFirstDayOfMonth(_query);
                yield return new OrderEndDistrubutionShouldBeLastSecondOfMonth(_query);
                yield return new OrderRequiredFieldsShouldBeSpecified(_query);
                yield return new OrderScanShouldPresent(_query);
                yield return new OrderShouldHaveAtLeastOnePosition(_query);
                yield return new OrderShouldNotBeSignedBeforeBargain(_query);

                yield return new AccountShouldExist(_query);
                yield return new AccountBalanceShouldBePositive(_query);
                yield return new LockShouldNotExist(_query);

                yield return new AdvertisementCountPerCategoryShouldBeLimited(_query);
                yield return new AdvertisementCountPerThemeShouldBeLimited(_query);
                yield return new AssociatedPositionsGroupCount(_query);
                yield return new AssociatedPositionsGroupCount(_query);
                yield return new AssociatedPositionWithoutPrincipal(_query);
                yield return new ConflictingPrincipalPosition(_query);
                yield return new DeniedPositionsCheck(_query);
                yield return new LinkedObjectsMissedInPrincipals(_query);
                yield return new MaximumAdvertisementAmount(_query);
                yield return new MinimalAdvertisementRestrictionShouldBeSpecified(_query);
                yield return new MinimumAdvertisementAmount(_query);
                yield return new OrderPositionCorrespontToInactivePosition(_query);
                yield return new OrderPositionShouldCorrespontToActualPrice(_query);
                yield return new OrderPositionsShouldCorrespontToActualPrice(_query);
                yield return new SatisfiedPrincipalPositionDifferentOrder(_query);
            }
        }
    }
}
