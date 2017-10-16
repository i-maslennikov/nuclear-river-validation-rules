using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.CheckModes
{
    internal static class CheckModeRegistry
    {
        public static readonly IReadOnlyCollection<Tuple<MessageTypeCode, IReadOnlyDictionary<CheckMode, RuleSeverityLevel>>> Map =
            new[]
                {
                    Rule(MessageTypeCode.AccountBalanceShouldBePositive,
                         manualReportWithAccount: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.AccountShouldExist,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.OrderPositionAdvertisementMustBeCreated,
                         single: RuleSeverityLevel.Error,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.OrderPositionAdvertisementMustHaveAdvertisement,
                         single: RuleSeverityLevel.Warning,
                         manualReport: RuleSeverityLevel.Warning,
                         prerelease: RuleSeverityLevel.Warning,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.AdvantageousPurchasesBannerMustBeSoldInTheSameCategory,
                         single: RuleSeverityLevel.Error,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.BargainScanShouldPresent,
                         single: RuleSeverityLevel.Warning),

                    Rule(MessageTypeCode.BillsShouldBeCreated,
                         single: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.BillsSumShouldMatchOrder,
                         single: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.LegalPersonProfileBargainShouldNotBeExpired,
                         single: RuleSeverityLevel.Info),

                    Rule(MessageTypeCode.LegalPersonProfileWarrantyShouldNotBeExpired,
                         single: RuleSeverityLevel.Info),

                    Rule(MessageTypeCode.LegalPersonShouldHaveAtLeastOneProfile,
                         single: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.LinkedCategoryAsterixMayBelongToFirm,
                         single: RuleSeverityLevel.Info,
                         prerelease: RuleSeverityLevel.Info,
                         release: RuleSeverityLevel.Info),

                    Rule(MessageTypeCode.LinkedCategoryFirmAddressShouldBeValid,
                         single: RuleSeverityLevel.Warning,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.LinkedCategoryShouldBeActive,
                         single: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.LinkedCategoryShouldBelongToFirm,
                         single: RuleSeverityLevel.Warning,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.LinkedFirmAddressShouldBeValid,
                         single: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.OrderBeginDistrubutionShouldBeFirstDayOfMonth,
                         single: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.OrderEndDistrubutionShouldBeLastSecondOfMonth,
                         single: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.OrderMustHaveActiveDeal,
                         single: RuleSeverityLevel.Error,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Warning,
                         release: RuleSeverityLevel.Warning),

                    Rule(MessageTypeCode.OrderMustHaveActiveLegalEntities,
                         single: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.OrderRequiredFieldsShouldBeSpecified,
                         single: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.OrderScanShouldPresent,
                         single: RuleSeverityLevel.Warning),

                    Rule(MessageTypeCode.OrderShouldHaveAtLeastOnePosition,
                         single: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.OrderShouldNotBeSignedBeforeBargain,
                         single: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.FirmAndOrderShouldBelongTheSameOrganizationUnit,
                         single: RuleSeverityLevel.Error,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.FirmShouldHaveLimitedCategoryCount,
                         single: RuleSeverityLevel.Warning,
                         manualReport: RuleSeverityLevel.Warning,
                         prerelease: RuleSeverityLevel.Warning),

                    Rule(MessageTypeCode.FirmWithSelfAdvMustHaveOnlyDesktopOrIndependentPositions,
                         single: RuleSeverityLevel.Error,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.LinkedFirmShouldBeValid,
                         single: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    // TODO: согласовать ошибку при всех типах проверок.
                    // сейчас повторяется логика erm, но мне она кажется странной, ошибка должна быть на всех уровнях - нельзя пропускать лишний заказ в ядро.
                    // например, аналогичная проверка на количество тематик в выпуске - выдает всегда ошибку.
                    Rule(MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited,
                         single: RuleSeverityLevel.Warning,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.AdvertisementCountPerThemeShouldBeLimited,
                         single: RuleSeverityLevel.Error,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.AssociatedPositionsGroupCount,
                         single: RuleSeverityLevel.Warning,
                         singleForApprove: RuleSeverityLevel.Error,
                         manualReport: RuleSeverityLevel.Warning,
                         prerelease: RuleSeverityLevel.Warning,
                         release: RuleSeverityLevel.Warning),

                    Rule(MessageTypeCode.FirmAssociatedPositionMustHavePrincipal,
                         single: RuleSeverityLevel.Error,
                         singleForApprove: RuleSeverityLevel.Error,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.FirmAssociatedPositionMustHavePrincipalWithDifferentBindingObject,
                         single: RuleSeverityLevel.Error,
                         singleForApprove: RuleSeverityLevel.Error,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.FirmAssociatedPositionMustHavePrincipalWithMatchedBindingObject,
                         single: RuleSeverityLevel.Error,
                         singleForApprove: RuleSeverityLevel.Error,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.FirmAssociatedPositionShouldNotStayAlone,
                         singleForCancel: RuleSeverityLevel.Warning),

                    Rule(MessageTypeCode.FirmPositionMustNotHaveDeniedPositions,
                         single: RuleSeverityLevel.Error,
                         singleForApprove: RuleSeverityLevel.Error,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.MinimalAdvertisementRestrictionShouldBeSpecified,
                         single: RuleSeverityLevel.Warning,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.AdvertisementAmountShouldMeetMaximumRestrictions,
                         single: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.AdvertisementAmountShouldMeetMinimumRestrictions,
                         single: RuleSeverityLevel.Warning),

                    Rule(MessageTypeCode.AdvertisementAmountShouldMeetMinimumRestrictionsMass,
                         prerelease: RuleSeverityLevel.Warning,
                         release: RuleSeverityLevel.Warning),

                    Rule(MessageTypeCode.OrderPositionCorrespontToInactivePosition,
                         single: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.OrderPositionMayCorrespontToActualPrice,
                         single: RuleSeverityLevel.Warning),

                    Rule(MessageTypeCode.OrderPositionMustCorrespontToActualPrice,
                         single: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.OrderMustHaveActualPrice,
                         single: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.FirmAddressMustBeLocatedOnTheMap,
                         single: RuleSeverityLevel.Error,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.OrderMustNotIncludeReleasedPeriod,
                         single: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.OrderMustUseCategoriesOnlyAvailableInProject,
                         single: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.OrderPositionCostPerClickMustBeSpecified,
                         single: RuleSeverityLevel.Error,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.OrderPositionCostPerClickMustNotBeLessMinimum,
                         single: RuleSeverityLevel.Error,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.OrderPositionSalesModelMustMatchCategorySalesModel,
                         single: RuleSeverityLevel.Error,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.ProjectMustContainCostPerClickMinimumRestriction,
                         single: RuleSeverityLevel.Error,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.DefaultThemeMustBeExactlyOne,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.DefaultThemeMustHaveOnlySelfAds,
                         single: RuleSeverityLevel.Error,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.ThemeCategoryMustBeActiveAndNotDeleted,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.ThemePeriodMustContainOrderPeriod,
                         single: RuleSeverityLevel.Error,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.FirmAddressMustNotHaveMultipleCallToAction,
                         single: RuleSeverityLevel.Error,
                         singleForApprove: RuleSeverityLevel.Error,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.FirmAddressShouldNotHaveMultiplePartnerAdvertisement,
                         manualReport: RuleSeverityLevel.Warning,
                         prerelease: RuleSeverityLevel.Warning,
                         release: RuleSeverityLevel.Warning),

                    Rule(MessageTypeCode.AdvertiserMustBeNotifiedAboutPartnerAdvertisement,
                         manualReport: RuleSeverityLevel.Warning,
                         prerelease: RuleSeverityLevel.Warning,
                         release: RuleSeverityLevel.Warning),

                    Rule(MessageTypeCode.PartnerAdvertisementShouldNotBeSoldToAdvertiser,
                         manualReport: RuleSeverityLevel.Warning,
                         prerelease: RuleSeverityLevel.Warning,
                         release: RuleSeverityLevel.Warning),

                    Rule(MessageTypeCode.AdvertisementMustBelongToFirm,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.AdvertisementMustPassReview,
                         manualReport: RuleSeverityLevel.Error,
                         prerelease: RuleSeverityLevel.Error,
                         release: RuleSeverityLevel.Error),

                    Rule(MessageTypeCode.AdvertisementShouldNotHaveComments,
                        manualReport: RuleSeverityLevel.Warning,
                        prerelease: RuleSeverityLevel.Warning,
                        release: RuleSeverityLevel.Warning)
                };

        private static Tuple<MessageTypeCode, IReadOnlyDictionary<CheckMode, RuleSeverityLevel>> Rule(
            MessageTypeCode rule,
            RuleSeverityLevel? single = null,
            RuleSeverityLevel? singleForCancel = null,
            RuleSeverityLevel? singleForApprove = null,
            RuleSeverityLevel? manualReport = null,
            RuleSeverityLevel? manualReportWithAccount = null,
            RuleSeverityLevel? prerelease = null,
            RuleSeverityLevel? release = null)
        {
            if (manualReport.HasValue && !manualReportWithAccount.HasValue)
            {
                manualReportWithAccount = manualReport;
            }

            var values = new Dictionary<CheckMode, RuleSeverityLevel?>
                {
                    { CheckMode.Single, single },
                    { CheckMode.SingleForCancel, singleForCancel },
                    { CheckMode.SingleForApprove, singleForApprove },
                    { CheckMode.Manual, manualReport },
                    { CheckMode.ManualWithAccount, manualReportWithAccount },
                    { CheckMode.Prerelease, prerelease },
                    { CheckMode.Release, release },
                };

            return new Tuple<MessageTypeCode, IReadOnlyDictionary<CheckMode, RuleSeverityLevel>>(rule, values.Where(x => x.Value.HasValue).ToDictionary(x => x.Key, x => x.Value.Value));
        }
    }
}