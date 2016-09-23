namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.AdvertisementRules
{
    public sealed class OrderMustNotContainDummyAdvertisementMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderMustNotContainDummyAdvertisementMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderMustNotContainDummyAdvertisement;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var positionReference = message.ReadPositionReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Позиция {_linkFactory.CreateLink(positionReference)} содержит заглушку рекламного материала");
        }
    }
}