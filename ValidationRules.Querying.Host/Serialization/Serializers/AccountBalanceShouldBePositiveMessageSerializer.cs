using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class AccountBalanceShouldBePositiveMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AccountBalanceShouldBePositive;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var dto = validationResult.ReadAccountBalanceMessage();

            return new MessageSerializerResult(orderReference,
                $"Для оформления заказа недостаточно средств. Необходимо: {dto.Planned}. Имеется: {dto.Available}. Необходим лимит: {dto.Required}");
        }
    }
}