namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.AdvertisementRules
{
    public sealed class OrderPeriodMustContainAdvertisementPeriodMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderPeriodMustContainAdvertisementPeriodMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderPeriodMustContainAdvertisementPeriod;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var orderPositionReference = message.ReadOrderPositionReference();
            var advertisementReference = message.ReadAdvertisementReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Период размещения рекламного материала {_linkFactory.CreateLink(advertisementReference)}, выбранного в позиции {_linkFactory.CreateLink(orderPositionReference)} должен захватывать 5 дней от текущего месяца размещения");
        }
    }
}