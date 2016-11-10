using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class OrderPositionsShouldCorrespontToActualPriceMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionsShouldCorrespontToActualPrice;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageSerializerResult(
                orderReference,
                "Не найден действующий для заказа прайс-лист");
        }
    }
}