﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuClear.ValidationRules.Replication
{
    public enum MessageTypeCode
    {
        MaximumAdvertisementAmount = 1,
        MinimalAdvertisementRestrictionShouldBeSpecified = 2,
        OrderPositionsShouldCorrespontToActualPrice = 3,
        OrderPositionCorrespontToInactivePosition = 4,
        OrderPositionShouldCorrespontToActualPrice = 5,
        MinimumAdvertisementAmount = 6,
        AssociatedPositionsGroupCount = 7,
        DeniedPositionsCheck = 8,
        AssociatedPositionWithoutPrincipal = 9,
        LinkedObjectsMissedInPrincipals = 10,
        ConflictingPrincipalPosition = 11,
        SatisfiedPrincipalPositionDifferentOrder = 15,
        AdvertisementCountPerThemeShouldBeLimited = 16,
        AdvertisementCountPerCategoryShouldBeLimited = 17,

        AccountShouldExist = 12,
        LockShouldNotExist = 13,
        AccountBalanceShouldBePositive = 14,

        OrderBeginDistrubutionShouldBeFirstDayOfMonth = 18,
        OrderEndDistrubutionShouldBeLastSecondOfMonth = 19,
        LegalPersonProfileBargainShouldNotBeExpired = 20,
        LegalPersonProfileWarrantyShouldNotBeExpired = 21,
        BillsPeriodShouldMatchOrder = 22,
        OrderShouldNotBeSignedBeforeBargain = 23,
        LegalPersonShouldHaveAtLeastOneProfile = 24,
        OrderShouldHaveAtLeastOnePosition = 25,
        OrderScanShouldPresent = 26,
        BargainScanShouldPresent = 27,
        OrderRequiredFieldsShouldBeSpecified = 28,
        LinkedFirmAddressShouldBeValid = 29,
        LinkedCategoryFirmAddressShouldBeValid = 30,
        LinkedCategoryShouldBelongToFirm = 32,
        LinkedCategoryAsterixMayBelongToFirm = 33,
        LinkedCategoryShouldBeActive = 34,
        LinkedFirmShouldBeValid = 35,
        BillsSumShouldMatchOrder = 36,
        BillsShouldBeCreated = 37,
    }
}