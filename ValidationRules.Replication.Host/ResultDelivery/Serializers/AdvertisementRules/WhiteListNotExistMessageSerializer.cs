namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.AdvertisementRules
{
    public sealed class WhiteListNotExistMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public WhiteListNotExistMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.WhiteListNotExist;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var firmReference = message.ReadFirmReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Для фирмы {_linkFactory.CreateLink(firmReference)} не указан рекламный материал в белый список");
        }
    }
}