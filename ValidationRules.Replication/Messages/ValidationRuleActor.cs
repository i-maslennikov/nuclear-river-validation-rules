using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Messages
{
    public sealed class ValidationRuleActor : IActor
    {
        private readonly IQuery _query;
        private readonly IRepository<Version> _versionRepository;
        private readonly IBulkRepository<Version.ErmState> _ermStatesRepository;
        private readonly IBulkRepository<Version.ValidationResult> _validationResultRepository;
        private readonly Dictionary<MessageTypeCode, IValidationResultAccessor> _accessors;

        public ValidationRuleActor(IQuery query,
                                   IRepository<Version> versionRepository,
                                   IBulkRepository<Version.ErmState> ermStatesRepository,
                                   IBulkRepository<Version.ValidationResult> validationResultRepository)
        {
            _query = query;
            _versionRepository = versionRepository;
            _ermStatesRepository = ermStatesRepository;
            _validationResultRepository = validationResultRepository;
            _accessors = new ValidationRuleRegistry(query).CreateAccessors().ToDictionary(x => (MessageTypeCode)x.MessageTypeId, x => x);
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            // Общая идея: "результаты проверок соответствуют указанному состоянию ERM или более позднему"
            // Конкретные идеи:
            //  1. Набор ValidationResult для версии не меняется, таблица ValidationResult работает только на наполнение (за исключениям операции архивирования)
            //  2. Набор ErmStates для версии мутабелен, при этом может быть пустым (если мы уже обработали изменения, но пока не знаем, какой версии они соответствуют)
            //  3. Версия без изменений не создаётся.

            var currentVersion = _query.For<Version>().OrderByDescending(x => x.Id).Take(1).AsEnumerable().First().Id;
            var versionResult = new List<Version.ValidationResult>();

            foreach (var ruleCommands in commands.OfType<RecalculateValidationRuleCommand>().GroupBy(x => x.Rule))
            {
                using (Probe.Create($"Rule {ruleCommands.Key}"))
                {
                    var filter = CreateFilter(ruleCommands);
                    var versionRuleResult = CalculateValidationRuleChanges(currentVersion, ruleCommands.Key, filter);
                    versionResult.AddRange(versionRuleResult);
                }
            }

            var ermStates = commands.OfType<CreateNewVersionCommand>().SelectMany(x => x.States).Select(x => new Version.ErmState { Token = x });
            if (versionResult.Count > 0)
            {
                using (Probe.Create("Create New Version"))
                {
                    CreateVersion(currentVersion + 1, ermStates, versionResult);
                }
            }
            else
            {
                using (Probe.Create("Update Existing Version"))
                {
                    UpdateVersion(currentVersion, ermStates);
                }
            }

            return Array.Empty<IEvent>();
        }

        private Expression<Func<Version.ValidationResult, bool>> CreateFilter(IEnumerable<RecalculateValidationRuleCommand> commands)
        {
            var ids = Enumerable.Empty<long>();
            foreach (var command in commands)
            {
                var partialCommand = command as RecalculateValidationRulePartiallyCommand;
                if (partialCommand == null)
                {
                    return x => true;
                }

                ids = ids.Union(partialCommand.Filter);
            }

            return x => x.OrderId.HasValue && ids.Contains(x.OrderId.Value);
        }

        private IEnumerable<Version.ValidationResult> CalculateValidationRuleChanges(long version, MessageTypeCode ruleCode, Expression<Func<Version.ValidationResult, bool>> filter)
        {
            try
            {
                List<Version.ValidationResult> sourceObjects;
                List<Version.ValidationResult> destObjects;

                using (Probe.Create("Query Source"))
                using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    // Запрос к данным посылаем вне транзакции, иначе будет DTC
                    var accessor = _accessors[ruleCode];
                    sourceObjects = accessor.GetSource().Where(filter).ToList();
                    scope.Complete();
                }

                using (Probe.Create("Query Target"))
                {
                    destObjects = _query.For<Version.ValidationResult>().Where(x => x.MessageType == (int)ruleCode).Where(filter).GetValidationResults(version).ToList();
                }

                using (Probe.Create("Merge"))
                {
                    var mergeResult = MergeTool.Merge(sourceObjects, destObjects, ValidationResultEqualityComparer.Instance);
                    return mergeResult.Difference.Union(mergeResult.Complement.ApplyResolved());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при вычислении правила {ruleCode}", ex);
            }
        }

        private void CreateVersion(long id, IEnumerable<Version.ErmState> ermStates, IEnumerable<Version.ValidationResult> results)
        {
            var version = new Version { Id = id, Date = DateTime.UtcNow };
            _versionRepository.Add(version);
            _versionRepository.Save();

            _validationResultRepository.Create(results.ApplyVersionId(id));
            _ermStatesRepository.Create(ermStates.ApplyVersionId(id));
        }

        private void UpdateVersion(long id, IEnumerable<Version.ErmState> ermStates)
        {
            var version = new Version { Id = id, Date = DateTime.UtcNow };
            _versionRepository.Update(version);
            _versionRepository.Save();

            _ermStatesRepository.Create(ermStates.ApplyVersionId(id));
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
                    new BillsSumShouldMatchOrder(_query),
                    new BillsShouldBeCreated(_query),
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
                    new FirmWithSpecialCategoryShouldHaveSpecialPurchasesOrder(_query),

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
