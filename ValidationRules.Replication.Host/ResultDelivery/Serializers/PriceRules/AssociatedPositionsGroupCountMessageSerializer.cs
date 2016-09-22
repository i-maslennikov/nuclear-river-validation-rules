namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.PriceRules
{
    public sealed class AssociatedPositionsGroupCountMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public AssociatedPositionsGroupCountMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.AssociatedPositionsGroupCount;

        public LocalizedMessage Serialize(Message message)
        {
            var priceReference = message.ReadPriceReference();
            var pricePositionReference = message.ReadPricePositionReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Прайс-лист {_linkFactory.CreateLink(priceReference)}",
                                        $"В Позиции прайс-листа {_linkFactory.CreateLink(pricePositionReference)} содержится более одной группы сопутствующих позиций, что не поддерживается системой");
        }
    }
}