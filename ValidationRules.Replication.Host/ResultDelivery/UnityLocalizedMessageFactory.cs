using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.AccountRules;
using NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.AdvertisementRules;
using NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules;
using NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.PriceRules;
using NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.FirmRules;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery
{
    public sealed class UnityLocalizedMessageFactory
    {
        private static readonly IReadOnlyCollection<Type> SerializerTypes = new[]
            {
                typeof(AccountBalanceShouldBePositiveMessageSerializer),
                typeof(AccountShouldExistMessageSerializer),
                typeof(LockShouldNotExistMessageSerializer),

                // AdvertisementRules
                typeof(AdvertisementElementMustPassReviewMessageSerializer),
                typeof(AdvertisementMustBelongToFirmMessageSerializer),
                typeof(OrderPositionAdvertisementMustBeCreatedMessageSerializer),
                typeof(OrderMustNotContainDummyAdvertisementMessageSerializer),
                typeof(OrderMustHaveAdvertisementMessageSerializer),
                typeof(OrderPositionAdvertisementMustHaveAdvertisementMessageSerializer),
                typeof(OrderPositionMustNotReferenceDeletedAdvertisementMessageSerializer),
                typeof(WhiteListAdvertisementMustPresentMessageSerializer),
                typeof(WhiteListAdvertisementMayPresentMessageSerializer),

                typeof(BargainScanShouldPresentMessageSerializer),
                typeof(BillsPeriodShouldMatchOrderMessageSerializer),
                typeof(BillsShouldBeCreatedMessageSerializer),
                typeof(BillsSumShouldMatchOrderMessageSerializer),
                typeof(LegalPersonProfileBargainShouldNotBeExpiredMessageSerializer),
                typeof(LegalPersonProfileWarrantyShouldNotBeExpiredMessageSerializer),
                typeof(LegalPersonShouldHaveAtLeastOneProfileMessageSerializer),
                typeof(LinkedCategoryAsterixMayBelongToFirmMessageSerializer),
                typeof(LinkedCategoryFirmAddressShouldBeValidMessageSerializer),
                typeof(LinkedCategoryShouldBeActiveMessageSerializer),
                typeof(LinkedCategoryShouldBelongToFirmMessageSerializer),
                typeof(LinkedFirmAddressShouldBeValidMessageSerializer),
                typeof(LinkedFirmShouldBeValidMessageSerializer),
                typeof(OrderBeginDistrubutionShouldBeFirstDayOfMonthMessageSerializer),
                typeof(OrderEndDistrubutionShouldBeLastSecondOfMonthMessageSerializer),
                typeof(OrderRequiredFieldsShouldBeSpecifiedMessageSerializer),
                typeof(OrderScanShouldPresentMessageSerializer),
                typeof(OrderShouldHaveAtLeastOnePositionMessageSerializer),
                typeof(OrderShouldNotBeSignedBeforeBargainMessageSerializer),

                typeof(AdvertisementCountPerCategoryShouldBeLimitedMessageSerializer),
                typeof(AdvertisementCountPerThemeShouldBeLimitedMessageSerializer),
                typeof(AssociatedPositionsGroupCountMessageSerializer),
                typeof(AssociatedPositionWithoutPrincipalMessageSerializer),
                typeof(ConflictingPrincipalPositionMessageSerializer),
                typeof(DeniedPositionsCheckMessageSerializer),
                typeof(LinkedObjectsMissedInPrincipalsMessageSerializer),
                typeof(MaximumAdvertisementAmountMessageSerializer),
                typeof(MinimalAdvertisementRestrictionShouldBeSpecifiedMessageSerializer),
                typeof(MinimumAdvertisementAmountMessageSerializer),
                typeof(OrderPositionCorrespontToInactivePositionMessageSerializer),
                typeof(OrderPositionShouldCorrespontToActualPriceMessageSerializer),
                typeof(OrderPositionsShouldCorrespontToActualPriceMessageSerializer),
                typeof(SatisfiedPrincipalPositionDifferentOrderMessageSerializer),

                typeof(FirmAndOrderShouldBelongTheSameOrganizationUnitMessageSerializer),
                typeof(FirmShouldHaveLimitedCategoryCountMessageSerializer),
                typeof(FirmWithSpecialCategoryShouldHaveSpecialPurchasesMessageSerializer),
            };

        private readonly Dictionary<MessageTypeCode, IMessageSerializer> _serializers;

        public UnityLocalizedMessageFactory(IUnityContainer container)
        {
            _serializers = SerializerTypes.Select(x => (IMessageSerializer)container.Resolve(x)).ToDictionary(x => x.MessageType, x => x);
        }

        public LocalizedMessage Localize(Message result)
        {
            IMessageSerializer serializer;
            return _serializers.TryGetValue(result.MessageType, out serializer)
                       ? serializer.Serialize(result)
                       : null;
        }
    }
}