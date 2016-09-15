namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.AdvertisementRules
{
    public sealed class WhiteListExistMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public WhiteListExistMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.WhiteListExist;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var firmReference = message.ReadFirmReference();
            var advertisementReference = message.ReadAdvertisementReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Для фирмы {_linkFactory.CreateLink(firmReference)} в белый список выбран рекламный материал {_linkFactory.CreateLink(advertisementReference)}");
        }
    }
}