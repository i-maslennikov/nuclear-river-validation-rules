namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.AdvertisementRules
{
    public sealed class RequiredWhiteListNotMissingMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public RequiredWhiteListNotMissingMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.RequiredWhiteListNotMissing;

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