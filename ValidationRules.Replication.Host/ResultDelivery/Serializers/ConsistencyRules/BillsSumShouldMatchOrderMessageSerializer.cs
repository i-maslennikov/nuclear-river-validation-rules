namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules
{
    public sealed class BillsSumShouldMatchOrderMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public BillsSumShouldMatchOrderMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.BillsSumShouldMatchOrder;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Сумма по счетам не совпадает с планируемой суммой заказа");
        }
    }
}