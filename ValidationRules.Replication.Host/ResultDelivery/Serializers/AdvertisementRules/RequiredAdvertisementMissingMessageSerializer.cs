namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.AdvertisementRules
{
    public sealed class RequiredAdvertisementMissingMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public RequiredAdvertisementMissingMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.RequiredAdvertisementMissing;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var orderPositionReference = message.ReadOrderPositionReference();
            var positionReference = message.ReadPositionReference();

            if (orderPositionReference.Item2 == positionReference.Item2)
            {
                return new LocalizedMessage(message.GetLevel(),
                                            $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                            $"В позиции {_linkFactory.CreateLink(orderPositionReference)} необходимо указать рекламные материалы");
            }

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"В позиции {_linkFactory.CreateLink(orderPositionReference)} необходимо указать рекламные материалы для подпозиции {_linkFactory.CreateLink(positionReference)}");
        }
    }
}