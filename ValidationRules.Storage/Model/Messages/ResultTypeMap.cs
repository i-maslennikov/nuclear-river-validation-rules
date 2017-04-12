using System;
using System.Collections.Generic;
using System.Linq;

namespace NuClear.ValidationRules.Storage.Model.Messages
{
    // TODO: переименовать Mass=>Manual, MassPrerelease => Prerelease
    public static class ResultTypeMap
    {
        public static readonly IReadOnlyDictionary<CheckMode, Dictionary<MessageTypeCode, RuleSeverityLevel>> Map = new Dictionary<MessageTypeCode, Dictionary<CheckMode, RuleSeverityLevel>>()
            {
                // В erm эта проверка не вызывается при ручной проверке, только при сборке (в том числе бете)
                { MessageTypeCode.AccountBalanceShouldBePositive, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.None)
                                                                                      .WhenMassWithAccount(RuleSeverityLevel.Error)
                                                                                      .WhenMassPrerelease(RuleSeverityLevel.None)
                                                                                      .WhenMassRelease(RuleSeverityLevel.Error)) },

                // В erm эта проверка не вызывается при ручной проверке, только при сборке (в том числе бете)
                { MessageTypeCode.AccountShouldExist, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.None)
                                                                          .WhenMass(RuleSeverityLevel.None)
                                                                          .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                          .WhenMassRelease(RuleSeverityLevel.Error)) },

                // В erm эта проверка не вызывается при ручной проверке, только при сборке (в том числе бете)
                { MessageTypeCode.LockShouldNotExist, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.None)
                                                                          .WhenMass(RuleSeverityLevel.None)
                                                                          .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                          .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.AdvertisementElementMustPassReview, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Warning)
                                                                                          .WhenMass(RuleSeverityLevel.Error)
                                                                                          .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                          .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.AdvertisementMustBelongToFirm, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                     .WhenMass(RuleSeverityLevel.Error)
                                                                                     .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                     .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.AdvertisementWebsiteShouldNotBeFirmWebsite, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Warning)
                                                                                                  .WhenMass(RuleSeverityLevel.None)
                                                                                                  .WhenMassPrerelease(RuleSeverityLevel.None)
                                                                                                  .WhenMassRelease(RuleSeverityLevel.None)) },

                { MessageTypeCode.CouponMustBeSoldOnceAtTime, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                  .WhenMass(RuleSeverityLevel.Error)
                                                                                  .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                  .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.OrderCouponPeriodMustBeInRelease, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.None)
                                                                                        .WhenMass(RuleSeverityLevel.Error)
                                                                                        .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                        .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.OrderCouponPeriodMustNotBeLessFiveDays, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Warning)
                                                                                              .WhenMass(RuleSeverityLevel.None)
                                                                                              .WhenMassPrerelease(RuleSeverityLevel.None)
                                                                                              .WhenMassRelease(RuleSeverityLevel.None)) },

                { MessageTypeCode.OrderMustHaveAdvertisement, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Warning)
                                                                                  .WhenMass(RuleSeverityLevel.Error)
                                                                                  .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                  .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.OrderMustNotContainDummyAdvertisement, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Warning)
                                                                                             .WhenMass(RuleSeverityLevel.Warning)
                                                                                             .WhenMassPrerelease(RuleSeverityLevel.Warning)
                                                                                             .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.OrderPositionAdvertisementMustBeCreated, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                               .WhenMass(RuleSeverityLevel.Error)
                                                                                               .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                               .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.OrderPositionAdvertisementMustHaveAdvertisement, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Warning)
                                                                                                       .WhenMass(RuleSeverityLevel.Error)
                                                                                                       .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                                       .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.OrderPositionMustNotReferenceDeletedAdvertisement, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                                         .WhenMass(RuleSeverityLevel.Error)
                                                                                                         .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                                         .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.WhiteListAdvertisementMayPresent, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Info)
                                                                                        .WhenMass(RuleSeverityLevel.Info)
                                                                                        .WhenMassPrerelease(RuleSeverityLevel.Info)
                                                                                        .WhenMassRelease(RuleSeverityLevel.Info)) },

                { MessageTypeCode.WhiteListAdvertisementMustPresent, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Warning)
                                                                                         .WhenMass(RuleSeverityLevel.Warning)
                                                                                         .WhenMassPrerelease(RuleSeverityLevel.Warning)
                                                                                         .WhenMassRelease(RuleSeverityLevel.Error)) },

                // удалённая проверка
                { MessageTypeCode.AdvantageousPurchasesBannerMustBeSoldInTheSameCategory, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                                              .WhenMass(RuleSeverityLevel.Error)
                                                                                                              .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                                              .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.BargainScanShouldPresent, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Warning)
                                                                                .WhenMass(RuleSeverityLevel.None)
                                                                                .WhenMassPrerelease(RuleSeverityLevel.None)
                                                                                .WhenMassRelease(RuleSeverityLevel.None)) },

                { MessageTypeCode.BillsPeriodShouldMatchOrder, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                   .WhenMass(RuleSeverityLevel.None)
                                                                                   .WhenMassPrerelease(RuleSeverityLevel.None)
                                                                                   .WhenMassRelease(RuleSeverityLevel.None)) },

                { MessageTypeCode.BillsShouldBeCreated, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                            .WhenMass(RuleSeverityLevel.None)
                                                                            .WhenMassPrerelease(RuleSeverityLevel.None)
                                                                            .WhenMassRelease(RuleSeverityLevel.None)) },

                { MessageTypeCode.BillsSumShouldMatchOrder, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                .WhenMass(RuleSeverityLevel.None)
                                                                                .WhenMassPrerelease(RuleSeverityLevel.None)
                                                                                .WhenMassRelease(RuleSeverityLevel.None)) },

                { MessageTypeCode.LegalPersonProfileBargainShouldNotBeExpired, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Info)
                                                                                                   .WhenMass(RuleSeverityLevel.None)
                                                                                                   .WhenMassPrerelease(RuleSeverityLevel.None)
                                                                                                   .WhenMassRelease(RuleSeverityLevel.None)) },

                { MessageTypeCode.LegalPersonProfileWarrantyShouldNotBeExpired, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Info)
                                                                                                    .WhenMass(RuleSeverityLevel.None)
                                                                                                    .WhenMassPrerelease(RuleSeverityLevel.None)
                                                                                                    .WhenMassRelease(RuleSeverityLevel.None)) },

                { MessageTypeCode.LegalPersonShouldHaveAtLeastOneProfile, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                              .WhenMass(RuleSeverityLevel.None)
                                                                                              .WhenMassPrerelease(RuleSeverityLevel.None)
                                                                                              .WhenMassRelease(RuleSeverityLevel.None)) },

                { MessageTypeCode.LinkedCategoryAsterixMayBelongToFirm, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Info)
                                                                                            .WhenMass(RuleSeverityLevel.None)
                                                                                            .WhenMassPrerelease(RuleSeverityLevel.Info)
                                                                                            .WhenMassRelease(RuleSeverityLevel.Info)) },

                { MessageTypeCode.LinkedCategoryFirmAddressShouldBeValid, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Warning)
                                                                                              .WhenMass(RuleSeverityLevel.None)
                                                                                              .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                              .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.LinkedCategoryShouldBeActive, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                    .WhenMass(RuleSeverityLevel.None)
                                                                                    .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                    .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.LinkedCategoryShouldBelongToFirm, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Warning)
                                                                                        .WhenMass(RuleSeverityLevel.None)
                                                                                        .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                        .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.LinkedFirmAddressShouldBeValid, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                      .WhenMass(RuleSeverityLevel.None)
                                                                                      .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                      .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.OrderBeginDistrubutionShouldBeFirstDayOfMonth, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                                     .WhenMass(RuleSeverityLevel.None)
                                                                                                     .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                                     .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.OrderEndDistrubutionShouldBeLastSecondOfMonth, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                                     .WhenMass(RuleSeverityLevel.None)
                                                                                                     .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                                     .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.OrderMustHaveActiveDeal, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                               .WhenMass(RuleSeverityLevel.Error)
                                                                               .WhenMassPrerelease(RuleSeverityLevel.Warning)
                                                                               .WhenMassRelease(RuleSeverityLevel.Warning)) },

                { MessageTypeCode.OrderMustHaveActiveLegalEntities, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                        .WhenMass(RuleSeverityLevel.None)
                                                                                        .WhenMassPrerelease(RuleSeverityLevel.None)
                                                                                        .WhenMassRelease(RuleSeverityLevel.None)) },

                { MessageTypeCode.OrderRequiredFieldsShouldBeSpecified, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                            .WhenMass(RuleSeverityLevel.None)
                                                                                            .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                            .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.OrderScanShouldPresent, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Warning)
                                                                              .WhenMass(RuleSeverityLevel.None)
                                                                              .WhenMassPrerelease(RuleSeverityLevel.None)
                                                                              .WhenMassRelease(RuleSeverityLevel.None)) },

                { MessageTypeCode.OrderShouldHaveAtLeastOnePosition, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                         .WhenMass(RuleSeverityLevel.None)
                                                                                         .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                         .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.OrderShouldNotBeSignedBeforeBargain, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                           .WhenMass(RuleSeverityLevel.None)
                                                                                           .WhenMassPrerelease(RuleSeverityLevel.None)
                                                                                           .WhenMassRelease(RuleSeverityLevel.None)) },

                { MessageTypeCode.FirmAndOrderShouldBelongTheSameOrganizationUnit, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                                       .WhenMass(RuleSeverityLevel.Error)
                                                                                                       .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                                       .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.FirmShouldHaveLimitedCategoryCount, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Warning)
                                                                                          .WhenMass(RuleSeverityLevel.Warning)
                                                                                          .WhenMassPrerelease(RuleSeverityLevel.Warning)
                                                                                          .WhenMassRelease(RuleSeverityLevel.None)) },

                { MessageTypeCode.FirmWithSelfAdvMustHaveOnlyDesktopOrIndependentPositions, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                                                .WhenMass(RuleSeverityLevel.Error)
                                                                                                                .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                                                .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchases, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.None)
                                                                                                         .WhenMass(RuleSeverityLevel.Error)
                                                                                                         .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                                         .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchasesOrder, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Warning)
                                                                                                              .WhenMass(RuleSeverityLevel.None)
                                                                                                              .WhenMassPrerelease(RuleSeverityLevel.None)
                                                                                                              .WhenMassRelease(RuleSeverityLevel.None)) },

                { MessageTypeCode.LinkedFirmShouldBeValid, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                               .WhenMass(RuleSeverityLevel.None)
                                                                               .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                               .WhenMassRelease(RuleSeverityLevel.Error)) },

                // TODO: согласовать ошибку при всех типах проверок.
                // сейчас повторяется логика erm, но мне она кажется странной, ошибка должна быть на всех уровнях - нельзя пропускать лишний заказ в ядро.
                // например, аналогичная проверка на количество тематик в выпуске - выдает всегда ошибку.
                { MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Warning)
                                                                                                    .WhenMass(RuleSeverityLevel.Error)
                                                                                                    .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                                    .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.AdvertisementCountPerThemeShouldBeLimited, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                                 .WhenMass(RuleSeverityLevel.Error)
                                                                                                 .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                                 .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.AssociatedPositionsGroupCount, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Warning)
                                                                                     .WhenSingleForApprove(RuleSeverityLevel.Error)
                                                                                     .WhenMass(RuleSeverityLevel.Warning)
                                                                                     .WhenMassPrerelease(RuleSeverityLevel.Warning)
                                                                                     .WhenMassRelease(RuleSeverityLevel.Warning)) },

                { MessageTypeCode.FirmAssociatedPositionMustHavePrincipal, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                               .WhenSingleForApprove(RuleSeverityLevel.Error)
                                                                                               .WhenMass(RuleSeverityLevel.Error)
                                                                                               .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                               .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.FirmAssociatedPositionMustHavePrincipalWithDifferentBindingObject, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                                                         .WhenSingleForApprove(RuleSeverityLevel.Error)
                                                                                                                         .WhenMass(RuleSeverityLevel.Error)
                                                                                                                         .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                                                         .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.FirmAssociatedPositionMustHavePrincipalWithMatchedBindingObject, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                                                       .WhenSingleForApprove(RuleSeverityLevel.Error)
                                                                                                                       .WhenMass(RuleSeverityLevel.Error)
                                                                                                                       .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                                                       .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.FirmAssociatedPositionShouldNotStayAlone, ResultBuilder(x => x.WhenSingleForCancel(RuleSeverityLevel.Warning)) },

                { MessageTypeCode.FirmPositionMustNotHaveDeniedPositions, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                              .WhenSingleForApprove(RuleSeverityLevel.Error)
                                                                                              .WhenMass(RuleSeverityLevel.Error)
                                                                                              .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                              .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.MaximumAdvertisementAmount, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                  .WhenMass(RuleSeverityLevel.None)
                                                                                  .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                  .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.MinimalAdvertisementRestrictionShouldBeSpecified, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Warning)
                                                                                                        .WhenMass(RuleSeverityLevel.None)
                                                                                                        .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                                        .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.MinimumAdvertisementAmount, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Warning)
                                                                                  .WhenMass(RuleSeverityLevel.None)
                                                                                  .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                  .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.OrderPositionCorrespontToInactivePosition, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                                 .WhenMass(RuleSeverityLevel.None)
                                                                                                 .WhenMassPrerelease(RuleSeverityLevel.None)
                                                                                                 .WhenMassRelease(RuleSeverityLevel.None)) },

                { MessageTypeCode.OrderPositionMayCorrespontToActualPrice, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Warning)
                                                                                               .WhenMass(RuleSeverityLevel.None)
                                                                                               .WhenMassPrerelease(RuleSeverityLevel.None)
                                                                                               .WhenMassRelease(RuleSeverityLevel.None)) },

                { MessageTypeCode.OrderPositionMustCorrespontToActualPrice, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                                .WhenMass(RuleSeverityLevel.None)
                                                                                                .WhenMassPrerelease(RuleSeverityLevel.None)
                                                                                                .WhenMassRelease(RuleSeverityLevel.None)) },

                { MessageTypeCode.OrderMustHaveActualPrice, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                                   .WhenMass(RuleSeverityLevel.None)
                                                                                                   .WhenMassPrerelease(RuleSeverityLevel.None)
                                                                                                   .WhenMassRelease(RuleSeverityLevel.None)) },


                { MessageTypeCode.FirmAddressMustBeLocatedOnTheMap, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                        .WhenMass(RuleSeverityLevel.Error)
                                                                                        .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                        .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.OrderMustNotIncludeReleasedPeriod, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                         .WhenMass(RuleSeverityLevel.None)
                                                                                         .WhenMassPrerelease(RuleSeverityLevel.None)
                                                                                         .WhenMassRelease(RuleSeverityLevel.None)) },

                { MessageTypeCode.OrderMustUseCategoriesOnlyAvailableInProject, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                                    .WhenMass(RuleSeverityLevel.None)
                                                                                                    .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                                    .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.OrderPositionCostPerClickMustBeSpecified, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                                .WhenMass(RuleSeverityLevel.Error)
                                                                                                .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                                .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.OrderPositionCostPerClickMustNotBeLessMinimum, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                                     .WhenMass(RuleSeverityLevel.Error)
                                                                                                     .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                                     .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.OrderPositionSalesModelMustMatchCategorySalesModel, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                                          .WhenMass(RuleSeverityLevel.Error)
                                                                                                          .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                                          .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.ProjectMustContainCostPerClickMinimumRestriction, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                                        .WhenMass(RuleSeverityLevel.Error)
                                                                                                        .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                                        .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.DefaultThemeMustBeExactlyOne, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.None)
                                                                                    .WhenMass(RuleSeverityLevel.Error)
                                                                                    .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                    .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.DefaultThemeMustHaveOnlySelfAds, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                       .WhenMass(RuleSeverityLevel.Error)
                                                                                       .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                       .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.ThemeCategoryMustBeActiveAndNotDeleted, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.None)
                                                                                              .WhenMass(RuleSeverityLevel.Error)
                                                                                              .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                              .WhenMassRelease(RuleSeverityLevel.Error)) },

                { MessageTypeCode.ThemePeriodMustContainOrderPeriod, ResultBuilder(x => x.WhenSingle(RuleSeverityLevel.Error)
                                                                                         .WhenMass(RuleSeverityLevel.Error)
                                                                                         .WhenMassPrerelease(RuleSeverityLevel.Error)
                                                                                         .WhenMassRelease(RuleSeverityLevel.Error)) },
        }.ToResultTypeMap();

        private static Dictionary<CheckMode, RuleSeverityLevel> ResultBuilder(Func<Dictionary<CheckMode, RuleSeverityLevel>, Dictionary<CheckMode, RuleSeverityLevel>> action)
        {
            return action(new Dictionary<CheckMode, RuleSeverityLevel>());
        }

        // TODO: WhenMass => WhenManual и т.д.
        private static Dictionary<CheckMode, RuleSeverityLevel> WhenSingle(this Dictionary<CheckMode, RuleSeverityLevel> map, RuleSeverityLevel ruleSeverityLevel) =>
            map.AddResult(CheckMode.Single, ruleSeverityLevel);
        private static Dictionary<CheckMode, RuleSeverityLevel> WhenSingleForCancel(this Dictionary<CheckMode, RuleSeverityLevel> map, RuleSeverityLevel ruleSeverityLevel) =>
            map.AddResult(CheckMode.SingleForCancel, ruleSeverityLevel);
        private static Dictionary<CheckMode, RuleSeverityLevel> WhenSingleForApprove(this Dictionary<CheckMode, RuleSeverityLevel> map, RuleSeverityLevel ruleSeverityLevel) =>
            map.AddResult(CheckMode.SingleForApprove, ruleSeverityLevel);
        private static Dictionary<CheckMode, RuleSeverityLevel> WhenMass(this Dictionary<CheckMode, RuleSeverityLevel> map, RuleSeverityLevel ruleSeverityLevel) =>
            map.AddResult(CheckMode.Manual, ruleSeverityLevel)
               .AddResult(CheckMode.ManualWithAccount, ruleSeverityLevel);
        private static Dictionary<CheckMode, RuleSeverityLevel> WhenMassWithAccount(this Dictionary<CheckMode, RuleSeverityLevel> map, RuleSeverityLevel ruleSeverityLevel) =>
            map.AddResult(CheckMode.ManualWithAccount, ruleSeverityLevel);
        private static Dictionary<CheckMode, RuleSeverityLevel> WhenMassPrerelease(this Dictionary<CheckMode, RuleSeverityLevel> map, RuleSeverityLevel ruleSeverityLevel) =>
            map.AddResult(CheckMode.Prerelease, ruleSeverityLevel);
        private static Dictionary<CheckMode, RuleSeverityLevel> WhenMassRelease(this Dictionary<CheckMode, RuleSeverityLevel> map, RuleSeverityLevel ruleSeverityLevel) =>
            map.AddResult(CheckMode.Release, ruleSeverityLevel);

        private static Dictionary<CheckMode, RuleSeverityLevel> AddResult(this Dictionary<CheckMode, RuleSeverityLevel> map, CheckMode checkMode, RuleSeverityLevel ruleSeverityLevel)
        {
            // не храним None
            if (ruleSeverityLevel != RuleSeverityLevel.None)
            {
                map.Add(checkMode, ruleSeverityLevel);
            }
            return map;
        }

        private static Dictionary<CheckMode, Dictionary<MessageTypeCode, RuleSeverityLevel>> ToResultTypeMap(this Dictionary<MessageTypeCode, Dictionary<CheckMode, RuleSeverityLevel>> messageTypeMap)
        {
            return messageTypeMap.Aggregate(new Dictionary<CheckMode, Dictionary<MessageTypeCode, RuleSeverityLevel>>(),
            (resultTypeMap, messageTypePair) =>
            {
                foreach (var resultTypePair in messageTypePair.Value)
                {
                    Dictionary<MessageTypeCode, RuleSeverityLevel> temp;
                    if (!resultTypeMap.TryGetValue(resultTypePair.Key, out temp))
                    {
                        temp = new Dictionary<MessageTypeCode, RuleSeverityLevel>();
                        resultTypeMap.Add(resultTypePair.Key, temp);
                    }

                    temp.Add(messageTypePair.Key, resultTypePair.Value);
                }

                return resultTypeMap;
            });
        }
    }
}