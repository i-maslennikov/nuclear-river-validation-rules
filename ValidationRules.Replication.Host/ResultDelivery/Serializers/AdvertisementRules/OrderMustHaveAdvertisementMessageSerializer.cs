namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.AdvertisementRules
{
    public sealed class OrderMustHaveAdvertisementMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderMustHaveAdvertisementMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderMustHaveAdvertisement;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var advertisementReference = message.ReadAdvertisementReference();
            var advertisementElementReference = message.ReadAdvertisementElementReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"В рекламном материале {_linkFactory.CreateLink(advertisementReference)} не заполнен обязательный элемент {_linkFactory.CreateLink(advertisementElementReference)}");
        }
    }
}