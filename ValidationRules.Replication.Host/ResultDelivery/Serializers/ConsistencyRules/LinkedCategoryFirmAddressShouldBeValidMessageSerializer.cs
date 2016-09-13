namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules
{
    public sealed class LinkedCategoryFirmAddressShouldBeValidMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public LinkedCategoryFirmAddressShouldBeValidMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.LinkedCategoryFirmAddressShouldBeValid;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var orderPositionReference = message.ReadOrderPositionReference();
            var categoryReference = message.ReadCategoryReference();
            var firmAddressReference = message.ReadFirmAddressReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"В позиции {_linkFactory.CreateLink(orderPositionReference)} найдена рубрика {_linkFactory.CreateLink(categoryReference)}, не принадлежащая адресу {_linkFactory.CreateLink(firmAddressReference)}");
        }
    }
}