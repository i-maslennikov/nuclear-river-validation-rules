using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class BillsPeriodShouldMatchOrderMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.BillsPeriodShouldMatchOrder;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageSerializerResult(orderReference, "Период размещения, указанный в заказе и в счете не совпадают");
        }
    }
}