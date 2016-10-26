namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.AdvertisementRules
{
    public sealed class AdvertisementWebsiteShouldNotBeFirmWebsiteMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public AdvertisementWebsiteShouldNotBeFirmWebsiteMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementWebsiteShouldNotBeFirmWebsite;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var firmReference = message.ReadFirmReference();
            var orderPositionReference = message.ReadOrderPositionReference();
            var website = message.ReadWebsite();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Для фирмы {_linkFactory.CreateLink(firmReference)} заказана рекламная ссылка {website} позиция {_linkFactory.CreateLink(orderPositionReference)}, дублирующая контакт фирмы");
        }
    }
}