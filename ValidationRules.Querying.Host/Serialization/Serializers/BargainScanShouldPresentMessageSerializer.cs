using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class BargainScanShouldPresentMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.BargainScanShouldPresent;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageSerializerResult(orderReference, "Отсутствует сканированная копия договора");
        }
    }
}