namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules
{
    public sealed class BillsPeriodShouldMatchOrderMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public BillsPeriodShouldMatchOrderMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.BillsPeriodShouldMatchOrder;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Период размещения, указанный в заказе и в счете не совпадают");
        }
    }
}