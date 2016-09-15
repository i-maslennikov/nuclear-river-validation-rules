namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.AdvertisementRules
{
    public sealed class AdvertisementElementDraftMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public AdvertisementElementDraftMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementElementDraft;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var advertisementReference = message.ReadAdvertisementReference();
            var advertisementElementReference = message.ReadAdvertisementElementReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"В рекламном материале {_linkFactory.CreateLink(advertisementReference)}, который подлежит выверке, элемент {_linkFactory.CreateLink(advertisementElementReference)} находится в статусе 'Черновик'");
        }
    }
}