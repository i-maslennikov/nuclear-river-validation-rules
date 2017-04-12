using System;
using System.Collections.Generic;
using System.Linq;

namespace NuClear.ValidationRules.Storage.Model.Messages
{
    // TODO: переименовать Mass=>Manual, MassPrerelease => Prerelease
    public static class ResultTypeMap
    {
        public static readonly IReadOnlyDictionary<ResultType, Dictionary<MessageTypeCode, Result>> Map = new Dictionary<MessageTypeCode, Dictionary<ResultType, Result>>()
            {
                // В erm эта проверка не вызывается при ручной проверке, только при сборке (в том числе бете)
                { MessageTypeCode.AccountBalanceShouldBePositive, ResultBuilder(x => x.WhenSingle(Result.None)
                                                                                      .WhenMassWithAccount(Result.Error)
                                                                                      .WhenMassPrerelease(Result.None)
                                                                                      .WhenMassRelease(Result.Error)) },

                // В erm эта проверка не вызывается при ручной проверке, только при сборке (в том числе бете)
                { MessageTypeCode.AccountShouldExist, ResultBuilder(x => x.WhenSingle(Result.None)
                                                                          .WhenMass(Result.None)
                                                                          .WhenMassPrerelease(Result.Error)
                                                                          .WhenMassRelease(Result.Error)) },

                // В erm эта проверка не вызывается при ручной проверке, только при сборке (в том числе бете)
                { MessageTypeCode.LockShouldNotExist, ResultBuilder(x => x.WhenSingle(Result.None)
                                                                          .WhenMass(Result.None)
                                                                          .WhenMassPrerelease(Result.Error)
                                                                          .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.AdvertisementElementMustPassReview, ResultBuilder(x => x.WhenSingle(Result.Warning)
                                                                                          .WhenMass(Result.Error)
                                                                                          .WhenMassPrerelease(Result.Error)
                                                                                          .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.AdvertisementMustBelongToFirm, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                     .WhenMass(Result.Error)
                                                                                     .WhenMassPrerelease(Result.Error)
                                                                                     .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.AdvertisementWebsiteShouldNotBeFirmWebsite, ResultBuilder(x => x.WhenSingle(Result.Warning)
                                                                                                  .WhenMass(Result.None)
                                                                                                  .WhenMassPrerelease(Result.None)
                                                                                                  .WhenMassRelease(Result.None)) },

                { MessageTypeCode.CouponMustBeSoldOnceAtTime, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                  .WhenMass(Result.Error)
                                                                                  .WhenMassPrerelease(Result.Error)
                                                                                  .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.OrderCouponPeriodMustBeInRelease, ResultBuilder(x => x.WhenSingle(Result.None)
                                                                                        .WhenMass(Result.Error)
                                                                                        .WhenMassPrerelease(Result.Error)
                                                                                        .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.OrderCouponPeriodMustNotBeLessFiveDays, ResultBuilder(x => x.WhenSingle(Result.Warning)
                                                                                              .WhenMass(Result.None)
                                                                                              .WhenMassPrerelease(Result.None)
                                                                                              .WhenMassRelease(Result.None)) },

                { MessageTypeCode.OrderMustHaveAdvertisement, ResultBuilder(x => x.WhenSingle(Result.Warning)
                                                                                  .WhenMass(Result.Error)
                                                                                  .WhenMassPrerelease(Result.Error)
                                                                                  .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.OrderMustNotContainDummyAdvertisement, ResultBuilder(x => x.WhenSingle(Result.Warning)
                                                                                             .WhenMass(Result.Warning)
                                                                                             .WhenMassPrerelease(Result.Warning)
                                                                                             .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.OrderPositionAdvertisementMustBeCreated, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                               .WhenMass(Result.Error)
                                                                                               .WhenMassPrerelease(Result.Error)
                                                                                               .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.OrderPositionAdvertisementMustHaveAdvertisement, ResultBuilder(x => x.WhenSingle(Result.Warning)
                                                                                                       .WhenMass(Result.Error)
                                                                                                       .WhenMassPrerelease(Result.Error)
                                                                                                       .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.OrderPositionMustNotReferenceDeletedAdvertisement, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                                         .WhenMass(Result.Error)
                                                                                                         .WhenMassPrerelease(Result.Error)
                                                                                                         .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.WhiteListAdvertisementMayPresent, ResultBuilder(x => x.WhenSingle(Result.Info)
                                                                                        .WhenMass(Result.Info)
                                                                                        .WhenMassPrerelease(Result.Info)
                                                                                        .WhenMassRelease(Result.Info)) },

                { MessageTypeCode.WhiteListAdvertisementMustPresent, ResultBuilder(x => x.WhenSingle(Result.Warning)
                                                                                         .WhenMass(Result.Warning)
                                                                                         .WhenMassPrerelease(Result.Warning)
                                                                                         .WhenMassRelease(Result.Error)) },

                // удалённая проверка
                { MessageTypeCode.AdvantageousPurchasesBannerMustBeSoldInTheSameCategory, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                                              .WhenMass(Result.Error)
                                                                                                              .WhenMassPrerelease(Result.Error)
                                                                                                              .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.BargainScanShouldPresent, ResultBuilder(x => x.WhenSingle(Result.Warning)
                                                                                .WhenMass(Result.None)
                                                                                .WhenMassPrerelease(Result.None)
                                                                                .WhenMassRelease(Result.None)) },

                { MessageTypeCode.BillsPeriodShouldMatchOrder, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                   .WhenMass(Result.None)
                                                                                   .WhenMassPrerelease(Result.None)
                                                                                   .WhenMassRelease(Result.None)) },

                { MessageTypeCode.BillsShouldBeCreated, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                            .WhenMass(Result.None)
                                                                            .WhenMassPrerelease(Result.None)
                                                                            .WhenMassRelease(Result.None)) },

                { MessageTypeCode.BillsSumShouldMatchOrder, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                .WhenMass(Result.None)
                                                                                .WhenMassPrerelease(Result.None)
                                                                                .WhenMassRelease(Result.None)) },

                { MessageTypeCode.LegalPersonProfileBargainShouldNotBeExpired, ResultBuilder(x => x.WhenSingle(Result.Info)
                                                                                                   .WhenMass(Result.None)
                                                                                                   .WhenMassPrerelease(Result.None)
                                                                                                   .WhenMassRelease(Result.None)) },

                { MessageTypeCode.LegalPersonProfileWarrantyShouldNotBeExpired, ResultBuilder(x => x.WhenSingle(Result.Info)
                                                                                                    .WhenMass(Result.None)
                                                                                                    .WhenMassPrerelease(Result.None)
                                                                                                    .WhenMassRelease(Result.None)) },

                { MessageTypeCode.LegalPersonShouldHaveAtLeastOneProfile, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                              .WhenMass(Result.None)
                                                                                              .WhenMassPrerelease(Result.None)
                                                                                              .WhenMassRelease(Result.None)) },

                { MessageTypeCode.LinkedCategoryAsterixMayBelongToFirm, ResultBuilder(x => x.WhenSingle(Result.Info)
                                                                                            .WhenMass(Result.None)
                                                                                            .WhenMassPrerelease(Result.Info)
                                                                                            .WhenMassRelease(Result.Info)) },

                { MessageTypeCode.LinkedCategoryFirmAddressShouldBeValid, ResultBuilder(x => x.WhenSingle(Result.Warning)
                                                                                              .WhenMass(Result.None)
                                                                                              .WhenMassPrerelease(Result.Error)
                                                                                              .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.LinkedCategoryShouldBeActive, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                    .WhenMass(Result.None)
                                                                                    .WhenMassPrerelease(Result.Error)
                                                                                    .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.LinkedCategoryShouldBelongToFirm, ResultBuilder(x => x.WhenSingle(Result.Warning)
                                                                                        .WhenMass(Result.None)
                                                                                        .WhenMassPrerelease(Result.Error)
                                                                                        .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.LinkedFirmAddressShouldBeValid, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                      .WhenMass(Result.None)
                                                                                      .WhenMassPrerelease(Result.Error)
                                                                                      .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.OrderBeginDistrubutionShouldBeFirstDayOfMonth, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                                     .WhenMass(Result.None)
                                                                                                     .WhenMassPrerelease(Result.Error)
                                                                                                     .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.OrderEndDistrubutionShouldBeLastSecondOfMonth, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                                     .WhenMass(Result.None)
                                                                                                     .WhenMassPrerelease(Result.Error)
                                                                                                     .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.OrderMustHaveActiveDeal, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                               .WhenMass(Result.Error)
                                                                               .WhenMassPrerelease(Result.Warning)
                                                                               .WhenMassRelease(Result.Warning)) },

                { MessageTypeCode.OrderMustHaveActiveLegalEntities, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                        .WhenMass(Result.None)
                                                                                        .WhenMassPrerelease(Result.None)
                                                                                        .WhenMassRelease(Result.None)) },

                { MessageTypeCode.OrderRequiredFieldsShouldBeSpecified, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                            .WhenMass(Result.None)
                                                                                            .WhenMassPrerelease(Result.Error)
                                                                                            .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.OrderScanShouldPresent, ResultBuilder(x => x.WhenSingle(Result.Warning)
                                                                              .WhenMass(Result.None)
                                                                              .WhenMassPrerelease(Result.None)
                                                                              .WhenMassRelease(Result.None)) },

                { MessageTypeCode.OrderShouldHaveAtLeastOnePosition, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                         .WhenMass(Result.None)
                                                                                         .WhenMassPrerelease(Result.Error)
                                                                                         .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.OrderShouldNotBeSignedBeforeBargain, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                           .WhenMass(Result.None)
                                                                                           .WhenMassPrerelease(Result.None)
                                                                                           .WhenMassRelease(Result.None)) },

                { MessageTypeCode.FirmAndOrderShouldBelongTheSameOrganizationUnit, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                                       .WhenMass(Result.Error)
                                                                                                       .WhenMassPrerelease(Result.Error)
                                                                                                       .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.FirmShouldHaveLimitedCategoryCount, ResultBuilder(x => x.WhenSingle(Result.Warning)
                                                                                          .WhenMass(Result.Warning)
                                                                                          .WhenMassPrerelease(Result.Warning)
                                                                                          .WhenMassRelease(Result.None)) },

                { MessageTypeCode.FirmWithSelfAdvMustHaveOnlyDesktopOrIndependentPositions, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                                                .WhenMass(Result.Error)
                                                                                                                .WhenMassPrerelease(Result.Error)
                                                                                                                .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchases, ResultBuilder(x => x.WhenSingle(Result.None)
                                                                                                         .WhenMass(Result.Error)
                                                                                                         .WhenMassPrerelease(Result.Error)
                                                                                                         .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchasesOrder, ResultBuilder(x => x.WhenSingle(Result.Warning)
                                                                                                              .WhenMass(Result.None)
                                                                                                              .WhenMassPrerelease(Result.None)
                                                                                                              .WhenMassRelease(Result.None)) },

                { MessageTypeCode.LinkedFirmShouldBeValid, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                               .WhenMass(Result.None)
                                                                               .WhenMassPrerelease(Result.Error)
                                                                               .WhenMassRelease(Result.Error)) },

                // TODO: согласовать ошибку при всех типах проверок.
                // сейчас повторяется логика erm, но мне она кажется странной, ошибка должна быть на всех уровнях - нельзя пропускать лишний заказ в ядро.
                // например, аналогичная проверка на количество тематик в выпуске - выдает всегда ошибку.
                { MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited, ResultBuilder(x => x.WhenSingle(Result.Warning)
                                                                                                    .WhenMass(Result.Error)
                                                                                                    .WhenMassPrerelease(Result.Error)
                                                                                                    .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.AdvertisementCountPerThemeShouldBeLimited, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                                 .WhenMass(Result.Error)
                                                                                                 .WhenMassPrerelease(Result.Error)
                                                                                                 .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.AssociatedPositionsGroupCount, ResultBuilder(x => x.WhenSingle(Result.Warning)
                                                                                     .WhenSingleForApprove(Result.Error)
                                                                                     .WhenMass(Result.Warning)
                                                                                     .WhenMassPrerelease(Result.Warning)
                                                                                     .WhenMassRelease(Result.Warning)) },

                { MessageTypeCode.FirmAssociatedPositionMustHavePrincipal, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                               .WhenSingleForApprove(Result.Error)
                                                                                               .WhenMass(Result.Error)
                                                                                               .WhenMassPrerelease(Result.Error)
                                                                                               .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.FirmAssociatedPositionMustHavePrincipalWithDifferentBindingObject, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                                                         .WhenSingleForApprove(Result.Error)
                                                                                                                         .WhenMass(Result.Error)
                                                                                                                         .WhenMassPrerelease(Result.Error)
                                                                                                                         .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.FirmAssociatedPositionMustHavePrincipalWithMatchedBindingObject, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                                                       .WhenSingleForApprove(Result.Error)
                                                                                                                       .WhenMass(Result.Error)
                                                                                                                       .WhenMassPrerelease(Result.Error)
                                                                                                                       .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.FirmAssociatedPositionShouldNotStayAlone, ResultBuilder(x => x.WhenSingleForCancel(Result.Warning)) },

                { MessageTypeCode.FirmPositionMustNotHaveDeniedPositions, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                              .WhenSingleForApprove(Result.Error)
                                                                                              .WhenMass(Result.Error)
                                                                                              .WhenMassPrerelease(Result.Error)
                                                                                              .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.MaximumAdvertisementAmount, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                  .WhenMass(Result.None)
                                                                                  .WhenMassPrerelease(Result.Error)
                                                                                  .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.MinimalAdvertisementRestrictionShouldBeSpecified, ResultBuilder(x => x.WhenSingle(Result.Warning)
                                                                                                        .WhenMass(Result.None)
                                                                                                        .WhenMassPrerelease(Result.Error)
                                                                                                        .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.MinimumAdvertisementAmount, ResultBuilder(x => x.WhenSingle(Result.Warning)
                                                                                  .WhenMass(Result.None)
                                                                                  .WhenMassPrerelease(Result.Error)
                                                                                  .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.OrderPositionCorrespontToInactivePosition, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                                 .WhenMass(Result.None)
                                                                                                 .WhenMassPrerelease(Result.None)
                                                                                                 .WhenMassRelease(Result.None)) },

                { MessageTypeCode.OrderPositionMayCorrespontToActualPrice, ResultBuilder(x => x.WhenSingle(Result.Warning)
                                                                                               .WhenMass(Result.None)
                                                                                               .WhenMassPrerelease(Result.None)
                                                                                               .WhenMassRelease(Result.None)) },

                { MessageTypeCode.OrderPositionMustCorrespontToActualPrice, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                                .WhenMass(Result.None)
                                                                                                .WhenMassPrerelease(Result.None)
                                                                                                .WhenMassRelease(Result.None)) },

                { MessageTypeCode.OrderMustHaveActualPrice, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                                   .WhenMass(Result.None)
                                                                                                   .WhenMassPrerelease(Result.None)
                                                                                                   .WhenMassRelease(Result.None)) },


                { MessageTypeCode.FirmAddressMustBeLocatedOnTheMap, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                        .WhenMass(Result.Error)
                                                                                        .WhenMassPrerelease(Result.Error)
                                                                                        .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.OrderMustNotIncludeReleasedPeriod, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                         .WhenMass(Result.None)
                                                                                         .WhenMassPrerelease(Result.None)
                                                                                         .WhenMassRelease(Result.None)) },

                { MessageTypeCode.OrderMustUseCategoriesOnlyAvailableInProject, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                                    .WhenMass(Result.None)
                                                                                                    .WhenMassPrerelease(Result.Error)
                                                                                                    .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.OrderPositionCostPerClickMustBeSpecified, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                                .WhenMass(Result.Error)
                                                                                                .WhenMassPrerelease(Result.Error)
                                                                                                .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.OrderPositionCostPerClickMustNotBeLessMinimum, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                                     .WhenMass(Result.Error)
                                                                                                     .WhenMassPrerelease(Result.Error)
                                                                                                     .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.OrderPositionSalesModelMustMatchCategorySalesModel, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                                          .WhenMass(Result.Error)
                                                                                                          .WhenMassPrerelease(Result.Error)
                                                                                                          .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.ProjectMustContainCostPerClickMinimumRestriction, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                                        .WhenMass(Result.Error)
                                                                                                        .WhenMassPrerelease(Result.Error)
                                                                                                        .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.DefaultThemeMustBeExactlyOne, ResultBuilder(x => x.WhenSingle(Result.None)
                                                                                    .WhenMass(Result.Error)
                                                                                    .WhenMassPrerelease(Result.Error)
                                                                                    .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.DefaultThemeMustHaveOnlySelfAds, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                       .WhenMass(Result.Error)
                                                                                       .WhenMassPrerelease(Result.Error)
                                                                                       .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.ThemeCategoryMustBeActiveAndNotDeleted, ResultBuilder(x => x.WhenSingle(Result.None)
                                                                                              .WhenMass(Result.Error)
                                                                                              .WhenMassPrerelease(Result.Error)
                                                                                              .WhenMassRelease(Result.Error)) },

                { MessageTypeCode.ThemePeriodMustContainOrderPeriod, ResultBuilder(x => x.WhenSingle(Result.Error)
                                                                                         .WhenMass(Result.Error)
                                                                                         .WhenMassPrerelease(Result.Error)
                                                                                         .WhenMassRelease(Result.Error)) },
        }.ToResultTypeMap();

        private static Dictionary<ResultType, Result> ResultBuilder(Func<Dictionary<ResultType, Result>, Dictionary<ResultType, Result>> action)
        {
            return action(new Dictionary<ResultType, Result>());
        }

        // TODO: WhenMass => WhenManual и т.д.
        private static Dictionary<ResultType, Result> WhenSingle(this Dictionary<ResultType, Result> map, Result result) =>
            map.AddResult(ResultType.Single, result);
        private static Dictionary<ResultType, Result> WhenSingleForCancel(this Dictionary<ResultType, Result> map, Result result) =>
            map.AddResult(ResultType.SingleForCancel, result);
        private static Dictionary<ResultType, Result> WhenSingleForApprove(this Dictionary<ResultType, Result> map, Result result) =>
            map.AddResult(ResultType.SingleForApprove, result);
        private static Dictionary<ResultType, Result> WhenMass(this Dictionary<ResultType, Result> map, Result result) =>
            map.AddResult(ResultType.Manual, result)
               .AddResult(ResultType.ManualWithAccount, result);
        private static Dictionary<ResultType, Result> WhenMassWithAccount(this Dictionary<ResultType, Result> map, Result result) =>
            map.AddResult(ResultType.ManualWithAccount, result);
        private static Dictionary<ResultType, Result> WhenMassPrerelease(this Dictionary<ResultType, Result> map, Result result) =>
            map.AddResult(ResultType.Prerelease, result);
        private static Dictionary<ResultType, Result> WhenMassRelease(this Dictionary<ResultType, Result> map, Result result) =>
            map.AddResult(ResultType.Release, result);

        private static Dictionary<ResultType, Result> AddResult(this Dictionary<ResultType, Result> map, ResultType resultType, Result result)
        {
            // не храним None
            if (resultType != ResultType.None && result != Result.None)
            {
                map.Add(resultType, result);
            }
            return map;
        }

        private static Dictionary<ResultType, Dictionary<MessageTypeCode, Result>> ToResultTypeMap(this Dictionary<MessageTypeCode, Dictionary<ResultType, Result>> messageTypeMap)
        {
            return messageTypeMap.Aggregate(new Dictionary<ResultType, Dictionary<MessageTypeCode, Result>>(),
            (resultTypeMap, messageTypePair) =>
            {
                foreach (var resultTypePair in messageTypePair.Value)
                {
                    Dictionary<MessageTypeCode, Result> temp;
                    if (!resultTypeMap.TryGetValue(resultTypePair.Key, out temp))
                    {
                        temp = new Dictionary<MessageTypeCode, Result>();
                        resultTypeMap.Add(resultTypePair.Key, temp);
                    }

                    temp.Add(messageTypePair.Key, resultTypePair.Value);
                }

                return resultTypeMap;
            });
        }
    }
}