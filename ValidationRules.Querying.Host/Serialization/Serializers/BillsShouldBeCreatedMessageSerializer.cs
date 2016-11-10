using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class BillsShouldBeCreatedMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.BillsShouldBeCreated;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageSerializerResult(orderReference, "Для заказа необходимо сформировать счета");
        }
    }
}