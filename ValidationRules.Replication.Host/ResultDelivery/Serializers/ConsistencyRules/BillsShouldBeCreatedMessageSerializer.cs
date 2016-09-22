namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules
{
    public sealed class BillsShouldBeCreatedMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public BillsShouldBeCreatedMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.BillsShouldBeCreated;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Для заказа необходимо сформировать счета");
        }
    }
}