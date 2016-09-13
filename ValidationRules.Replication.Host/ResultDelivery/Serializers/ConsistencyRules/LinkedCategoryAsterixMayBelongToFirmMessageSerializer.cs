namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules
{
    public sealed class LinkedCategoryAsterixMayBelongToFirmMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public LinkedCategoryAsterixMayBelongToFirmMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.LinkedCategoryAsterixMayBelongToFirm;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var orderPositionReference = message.ReadOrderPositionReference();
            var categoryReference = message.ReadCategoryReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"В позиции {_linkFactory.CreateLink(orderPositionReference)} найдена рубрика {_linkFactory.CreateLink(categoryReference)}, не принадлежащая фирме заказа");
        }
    }
}