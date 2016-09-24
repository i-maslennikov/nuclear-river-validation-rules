namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.FirmRules
{
    public sealed class FirmAndOrderShouldBelongTheSameOrganizationUnitMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public FirmAndOrderShouldBelongTheSameOrganizationUnitMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.FirmAndOrderShouldBelongTheSameOrganizationUnit;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Отделение организации назначения заказа не соответствует отделению организации выбранной фирмы");
        }
    }
}