using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderCouponPeriodInReleaseMustNotBeLessFiveDaysMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderCouponPeriodInReleaseMustNotBeLessFiveDays;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var advertisementReference = validationResult.ReadAdvertisementReference();

            return new MessageComposerResult(
                orderReference,
                Resources.AdvertisementPeriodEndsBeforeReleasePeriodBegins,
                advertisementReference,
                orderPositionReference);
        }
    }
}