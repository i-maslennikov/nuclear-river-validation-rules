namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.AccountRules
{
    public sealed class AccountShouldExistMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public AccountShouldExistMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.AccountShouldExist;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Заказ не имеет привязки к лицевому счёту");
        }
    }
}