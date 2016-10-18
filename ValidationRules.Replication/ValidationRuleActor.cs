using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Telemetry.Probing;
using NuClear.ValidationRules.Replication.AccountRules.Validation;
using NuClear.ValidationRules.Replication.AdvertisementRules.Validation;
using NuClear.ValidationRules.Replication.ConsistencyRules.Validation;
using NuClear.ValidationRules.Replication.FirmRules.Validation;
using NuClear.ValidationRules.Replication.PriceRules.Validation;
using NuClear.ValidationRules.Replication.ProjectRules.Validation;
using NuClear.ValidationRules.Replication.ThemeRules.Validation;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication
{
    public sealed class ValidationRuleActor : IActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<Version.ValidationResult> _repository;
        private readonly IBulkRepository<Version.ValidationResultForBulkDelete> _deleteRepository;
        private readonly ValidationRuleRegistry _registry;

        public ValidationRuleActor(IQuery query, IBulkRepository<Version.ValidationResult> repository, IBulkRepository<Version.ValidationResultForBulkDelete> deleteRepository)
        {
            _query = query;
            _repository = repository;
            _deleteRepository = deleteRepository;
            _registry = new ValidationRuleRegistry(query);
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var currentVersion = _query.For<Version>().OrderByDescending(x => x.Id).FirstOrDefault();
            var currentVersionId = currentVersion?.Id ?? 0;

            using (Probe.Create($"Delete"))
            {
                // Данные в целевых таблицах меняем в одной большой транзакции (сейчас она управляется из хендлера)
                var forBulkDelete = new Version.ValidationResultForBulkDelete { VersionId = currentVersionId };
                _deleteRepository.Delete(new[] { forBulkDelete });
            }

            return _registry.CreateAccessors().SelectMany(accessor => ProcessRule(accessor, currentVersionId)).ToArray();
        }

        public IReadOnlyCollection<IEvent> ProcessRule(IValidationResultAccessor accessor, long currentVersion)
        {
            using (Probe.Create($"Rule {accessor.GetType().Name}"))
            {
                IReadOnlyCollection<Version.ValidationResult> sourceObjects;
                using (Probe.Create($"Query"))
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
                    {
                        // Запрос к данным источника посылаем вне транзакции, большой беды от этого быть не должно.
                        var query = accessor.GetSource().ApplyVersion(currentVersion);
                        sourceObjects = query.ToArray();

                        // todo: удалить, добавлено с целью отладкиP
                        sourceObjects = sourceObjects.Where(x => x.PeriodStart >= DateTime.Parse("2016-06-01")).ToArray();

                        scope.Complete();
                    }
                }

                using (Probe.Create($"Create"))
                {
                    _repository.Create(sourceObjects);
                }

                return Array.Empty<IEvent>();
            }
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
                return new IValidationResultAccessor[]
                {
                    new BargainScanShouldPresent(_query),
                    new BillsPeriodShouldMatchOrder(_query),
                    new LegalPersonProfileBargainShouldNotBeExpired(_query),
                    new LegalPersonProfileWarrantyShouldNotBeExpired(_query),
                    new LegalPersonShouldHaveAtLeastOneProfile(_query),
                    new LinkedCategoryAsterixMayBelongToFirm(_query),
                    new LinkedCategoryFirmAddressShouldBeValid(_query),
                    new LinkedCategoryShouldBeActive(_query),
                    new LinkedCategoryShouldBelongToFirm(_query),
                    new LinkedFirmAddressShouldBeValid(_query),
                    new LinkedFirmShouldBeValid(_query),
                    new OrderBeginDistrubutionShouldBeFirstDayOfMonth(_query),
                    new OrderEndDistrubutionShouldBeLastSecondOfMonth(_query),
                    new OrderRequiredFieldsShouldBeSpecified(_query),
                    new OrderScanShouldPresent(_query),
                    new OrderShouldHaveAtLeastOnePosition(_query),
                    new OrderShouldNotBeSignedBeforeBargain(_query),

                    new AccountShouldExist(_query),
                    new AccountBalanceShouldBePositive(_query),
                    new LockShouldNotExist(_query),

                    // AdvertisementRules
                    new AdvertisementElementMustPassReview(_query),
                    new AdvertisementMustBelongToFirm(_query),
                    new OrderPositionAdvertisementMustBeCreated(_query),
                    new OrderMustNotContainDummyAdvertisement(_query),
                    new OrderMustHaveAdvertisement(_query),
                    new OrderPositionAdvertisementMustHaveAdvertisement(_query),
                    new OrderPositionMustNotReferenceDeletedAdvertisement(_query),
                    new WhiteListAdvertisementMustPresent(_query),
                    new WhiteListAdvertisementMayPresent(_query),

                    new AdvertisementCountPerCategoryShouldBeLimited(_query),
                    new AdvertisementCountPerThemeShouldBeLimited(_query),
                    new AssociatedPositionsGroupCount(_query),
                    new AssociatedPositionWithoutPrincipal(_query),
                    new ConflictingPrincipalPosition(_query),
                    new DeniedPositionsCheck(_query),
                    new LinkedObjectsMissedInPrincipals(_query),
                    new MaximumAdvertisementAmount(_query),
                    new MinimalAdvertisementRestrictionShouldBeSpecified(_query),
                    new MinimumAdvertisementAmount(_query),
                    new OrderPositionCorrespontToInactivePosition(_query),
                    new OrderPositionShouldCorrespontToActualPrice(_query),
                    new OrderPositionsShouldCorrespontToActualPrice(_query),
                    new SatisfiedPrincipalPositionDifferentOrder(_query),

                    new FirmAndOrderShouldBelongTheSameOrganizationUnit(_query),
                    new FirmShouldHaveLimitedCategoryCount(_query),
                    new FirmWithSpecialCategoryShouldHaveSpecialPurchases(_query),

                    new FirmAddressMustBeLocatedOnTheMap(_query),
                    new OrderMustNotIncludeReleasedPeriod(_query),
                    new OrderMustUseCategoriesOnlyAvailableInProject(_query),
                    new OrderPositionCostPerClickMustBeSpecified(_query),
                    new OrderPositionCostPerClickMustNotBeLessMinimum(_query),
                    new OrderPositionSalesModelMustMatchCategorySalesModel(_query),
                    new ProjectMustContainCostPerClickMinimumRestriction(_query),

                    // ThemeRules
                    new DefaultThemeMustBeExactlyOne(_query),
                    new ThemeCategoryMustBeActiveAndNotDeleted(_query),
                    new ThemePeriodMustContainOrderPeriod(_query),
                    new DefaultThemeMustHaveOnlySelfAds(_query),
                };
            }
        }
    }
}
