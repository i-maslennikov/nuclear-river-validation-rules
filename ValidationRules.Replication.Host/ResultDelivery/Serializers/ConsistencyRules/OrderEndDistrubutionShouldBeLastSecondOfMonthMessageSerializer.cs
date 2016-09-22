namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules
{
    public sealed class OrderEndDistrubutionShouldBeLastSecondOfMonthMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderEndDistrubutionShouldBeLastSecondOfMonthMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderEndDistrubutionShouldBeLastSecondOfMonth;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        "Указана некорректная дата окончания размещения");
        }
    }
}