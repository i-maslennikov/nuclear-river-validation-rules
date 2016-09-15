namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.AdvertisementRules
{
    public sealed class RemovedAdvertisemendSpecifiedForPositionMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public RemovedAdvertisemendSpecifiedForPositionMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.RemovedAdvertisemendSpecifiedForPosition;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var orderPositionReference = message.ReadOrderPositionReference();
            var advertisementReference = message.ReadAdvertisementReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"В позиции {_linkFactory.CreateLink(orderPositionReference)} выбран удалённый рекламный материал {_linkFactory.CreateLink(advertisementReference)}");
        }
    }
}