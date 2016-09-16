namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules
{
    public sealed class OrderScanShouldPresentMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderScanShouldPresentMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderScanShouldPresent;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();

            return new LocalizedMessage(message.GetLevel(),
                                    $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                    "Отсутствует сканированная копия Бланка заказа");
        }
    }
}