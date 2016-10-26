namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.PriceRules
{
    public sealed class CouponMustBeSoldOnceAtTimeMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public CouponMustBeSoldOnceAtTimeMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.CouponMustBeSoldOnceAtTime;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var advertisementReference = message.ReadAdvertisementReference();
            var orderPositionReference = message.ReadOrderPositionReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Купон на скидку {_linkFactory.CreateLink(advertisementReference)} прикреплён к позиции: {_linkFactory.CreateLink(orderPositionReference)}");
        }
    }
}