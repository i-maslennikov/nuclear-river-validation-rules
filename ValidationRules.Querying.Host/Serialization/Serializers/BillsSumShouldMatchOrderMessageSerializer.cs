using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class BillsSumShouldMatchOrderMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.BillsSumShouldMatchOrder;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageSerializerResult(orderReference, "Сумма по счетам не совпадает с планируемой суммой заказа");
        }
    }
}