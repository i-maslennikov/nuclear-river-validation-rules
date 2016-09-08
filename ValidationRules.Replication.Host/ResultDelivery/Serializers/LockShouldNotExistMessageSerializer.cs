namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class LockShouldNotExistMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public LockShouldNotExistMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.LockShouldNotExist;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Заказ имеет созданную блокировку на указанный период");
        }
    }
}