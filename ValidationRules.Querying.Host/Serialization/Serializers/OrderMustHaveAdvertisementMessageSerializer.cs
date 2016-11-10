using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class OrderMustHaveAdvertisementMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderMustHaveAdvertisement;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var advertisementReference = validationResult.ReadAdvertisementReference();
            var advertisementElementReference = validationResult.ReadAdvertisementElementReference();

            return new MessageSerializerResult(
                orderReference,
                "В рекламном материале {0} не заполнен обязательный элемент {1}",
                advertisementReference,
                advertisementElementReference);
        }
    }
}