namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.PriceRules
{
    public sealed class OrderPositionsShouldCorrespontToActualPriceMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderPositionsShouldCorrespontToActualPriceMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionsShouldCorrespontToActualPrice;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Не найден действующий для заказа прайс-лист");
        }
    }
}