namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.PriceRules
{
    public sealed class CouponMustBeUniqueForFirmMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public CouponMustBeUniqueForFirmMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.CouponMustBeUniqueForFirm;

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