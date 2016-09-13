namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules
{
    public sealed class LinkedCategoryShouldBeActiveMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public LinkedCategoryShouldBeActiveMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.LinkedCategoryShouldBeActive;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var orderPositionReference = message.ReadOrderPositionReference();
            var categoryReference = message.ReadCategoryReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"В позиции {_linkFactory.CreateLink(orderPositionReference)} найдена неактивная рубрика {_linkFactory.CreateLink(categoryReference)}");
        }
    }
}