namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules
{
    public sealed class OrderBeginDistrubutionShouldBeFirstDayOfMonthMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderBeginDistrubutionShouldBeFirstDayOfMonthMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderBeginDistrubutionShouldBeFirstDayOfMonth;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        "Указана некорректная дата начала размещения");
        }
    }
}