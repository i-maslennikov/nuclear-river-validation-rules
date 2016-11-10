using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class MinimalAdvertisementRestrictionShouldBeSpecifiedMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.MinimalAdvertisementRestrictionShouldBeSpecified;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var price = validationResult.ReadPriceReference();
            var pricePosition = validationResult.ReadPricePositionReference();

            return new MessageSerializerResult(
                price,
                "В позиции прайса {0} необходимо указать минимальное количество рекламы в выпуск",
                pricePosition);
        }
    }
}