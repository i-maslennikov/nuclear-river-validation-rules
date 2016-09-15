namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.AdvertisementRules
{
    public sealed class CompositePositionMustHaveAdvertisementsMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public CompositePositionMustHaveAdvertisementsMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.CompositePositionMustHaveAdvertisements;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var orderPositionReference = message.ReadOrderPositionReference();
            var positionReference = message.ReadPositionReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"В позиции {_linkFactory.CreateLink(orderPositionReference)} необходимо указать рекламные материалы для подпозиции {_linkFactory.CreateLink(positionReference)}");
        }
    }
}