namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules
{
    public sealed class OrderShouldNotBeSignedBeforeBargainMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderShouldNotBeSignedBeforeBargainMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderShouldNotBeSignedBeforeBargain;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();

            return new LocalizedMessage(message.GetLevel(),
                                    $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                    "Договор не может иметь дату подписания позднее даты подписания заказа");
        }
    }
}