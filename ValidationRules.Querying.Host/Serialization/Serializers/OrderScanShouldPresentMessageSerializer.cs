using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class OrderScanShouldPresentMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderScanShouldPresent;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageSerializerResult(
                orderReference,
                "Отсутствует сканированная копия Бланка заказа");
        }
    }
}