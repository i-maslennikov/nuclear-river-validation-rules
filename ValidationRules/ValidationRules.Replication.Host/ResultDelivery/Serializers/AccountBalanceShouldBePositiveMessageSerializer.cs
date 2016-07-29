using NuClear.ValidationRules.Replication.AccountRules.Validation;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class AccountBalanceShouldBePositiveMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public AccountBalanceShouldBePositiveMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public int MessageType
            => AccountBalanceShouldBePositiveActor.MessageTypeId;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var dto = message.ReadAccountBalanceMessage();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Для оформления заказа недостаточно средств. Необходимо: {dto.Planned}. Имеется: {dto.Available}. Необходим лимит: {dto.Required}");
        }
    }
}