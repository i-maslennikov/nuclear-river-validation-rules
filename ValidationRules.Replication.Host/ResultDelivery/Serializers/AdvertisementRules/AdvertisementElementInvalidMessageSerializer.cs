namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.AdvertisementRules
{
    public sealed class AdvertisementElementInvalidMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public AdvertisementElementInvalidMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementElementInvalid;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var advertisementReference = message.ReadAdvertisementReference();
            var advertisementElementReference = message.ReadAdvertisementElementReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"В рекламном материале {_linkFactory.CreateLink(advertisementReference)}, который подлежит выверке, элемент {_linkFactory.CreateLink(advertisementElementReference)} содержит ошибки выверки");
        }
    }
}