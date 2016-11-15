using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class MinimalAdvertisementRestrictionShouldBeSpecifiedMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.MinimalAdvertisementRestrictionShouldBeSpecified;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var price = validationResult.ReadPriceReference();
            var pricePosition = validationResult.ReadPricePositionReference();

            return new MessageComposerResult(
                price,
                "В позиции прайса {0} необходимо указать минимальное количество рекламы в выпуск",
                pricePosition);
        }
    }
}