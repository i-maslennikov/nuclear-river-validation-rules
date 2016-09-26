namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.AdvertisementRules
{
    public sealed class RequiredWhiteListMissingMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public RequiredWhiteListMissingMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.RequiredWhiteListMissing;

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