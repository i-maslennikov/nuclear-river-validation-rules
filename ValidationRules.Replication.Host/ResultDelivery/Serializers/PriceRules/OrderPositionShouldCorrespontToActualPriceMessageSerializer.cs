namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.PriceRules
{
    public sealed class OrderPositionShouldCorrespontToActualPriceMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderPositionShouldCorrespontToActualPriceMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionShouldCorrespontToActualPrice;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var orderPositionReference = message.ReadOrderPositionReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Позиция {_linkFactory.CreateLink(orderPositionReference)} не соответствует актуальному прайс-листу. Необходимо указать позицию из текущего действующего прайс-листа");
        }
    }
}
