using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Writings;
using NuClear.Telemetry.Probing;
using NuClear.ValidationRules.Replication.AccountRules.Validation;
using NuClear.ValidationRules.Replication.AdvertisementRules.Validation;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.ConsistencyRules.Validation;
using NuClear.ValidationRules.Replication.FirmRules.Validation;
using NuClear.ValidationRules.Replication.PriceRules.Validation;
using NuClear.ValidationRules.Replication.ProjectRules.Validation;
using NuClear.ValidationRules.Replication.ThemeRules.Validation;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Messages
{
    public sealed class ValidationRuleActor : IActor
    {
        private readonly IQuery _query;
        private readonly IRepository<Version> _versionRepository;
        private readonly IBulkRepository<Version.ErmState> _ermStatesRepository;
        private readonly IBulkRepository<Version.ValidationResult> _validationResultRepository;
        private readonly ValidationRuleRegistry _registry;

        public ValidationRuleActor(IQuery query,
                                   IRepository<Version> versionRepository,
                                   IBulkRepository<Version.ErmState> ermStatesRepository,
                                   IBulkRepository<Version.ValidationResult> validationResultRepository)
        {
            _query = query;
            _versionRepository = versionRepository;
            _ermStatesRepository = ermStatesRepository;
            _validationResultRepository = validationResultRepository;
            _registry = new ValidationRuleRegistry(query);
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var sourceObjects = Enumerable.Empty<Version.ValidationResult>();

            // Запрос к данным посылаем вне транзакции, иначе будет DTC
            using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                foreach (var accessor in _registry.CreateAccessors())
                {
                    using (Probe.Create($"Rule {accessor.GetType().Name} Query Source"))
                    {
                        var accessorSourceObjects = accessor.GetSource().ToList();
                        sourceObjects = sourceObjects.Concat(accessorSourceObjects);
                    }
                }

                scope.Complete();
            }

            var ermStates = commands.Cast<CreateNewVersionCommand>().SelectMany(x => x.States).Select(x => new Version.ErmState { Token = x });

            var currentVersion = _query.For<Version>().OrderByDescending(x => x.Id).Take(1).AsEnumerable().First();
            IReadOnlyCollection<Version.ValidationResult> validationResults;
            using (Probe.Create("Merge"))
            {
                var destObjects = _query.For<Version.ValidationResult>().GetValidationResults(currentVersion.Id);
                var mergeResult = MergeTool.Merge(sourceObjects, destObjects, ValidationResultEqualityComparer.Instance);

                validationResults = mergeResult.Difference.Union(mergeResult.Complement.ApplyResolved()).ToList();
            }

            using (Probe.Create("Create"))
            {
                if (validationResults.Count != 0)
                {
                    var newVersionId = currentVersion.Id + 1;

                    CreateVersion(newVersionId);
                    _ermStatesRepository.Create(ermStates.ApplyVersionId(newVersionId));
                    _validationResultRepository.Create(validationResults.ApplyVersionId(newVersionId));
                }
                else
                {
                    UpdateVersion(currentVersion.Id);
                    _ermStatesRepository.Create(ermStates.ApplyVersionId(currentVersion.Id));
                }
            }

            return Array.Empty<IEvent>();
        }

        private void CreateVersion(long id)
        {
            var version = new Version { Id = id, Date = DateTime.UtcNow };
            _versionRepository.Add(version);
            _versionRepository.Save();
        }

        private void UpdateVersion(long id)
        {
            var version = new Version { Id = id, Date = DateTime.UtcNow };
            _versionRepository.Update(version);
            _versionRepository.Save();
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
                    new AdvantageousPurchasesBannerMustBeSoldInTheSameCategory(_query),
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
                    new OrderMustHaveActiveDeal(_query),
                    new OrderMustHaveActiveLegalEntities(_query),
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
                    new CouponMustBeSoldOnceAtTime(_query),
                    new OrderPeriodMustContainAdvertisementPeriod(_query),
                    new AdvertisementWebsiteShouldNotBeFirmWebsite(_query),

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
                    new FirmWithSelfAdvMustHaveOnlyDesktopOrIndependentPositions(_query),
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
