using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class FirmWithSpecialCategoryShouldHaveSpecialPurchasesMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchases;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var firmReference = validationResult.ReadFirmReference();

            return new MessageComposerResult(
                firmReference,
                Resources.ThereIsNoAdvertisementForAdvantageousPurchasesCategory,
                firmReference);
        }
    }
}