namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.FirmRules
{
    public sealed class FirmShouldHaveLimitedCategoryCountMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public FirmShouldHaveLimitedCategoryCountMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.FirmShouldHaveLimitedCategoryCount;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var firmReference = message.ReadFirmReference();
            var categoryCount = message.ReadCategoryCount();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Для фирмы {_linkFactory.CreateLink(firmReference)} задано слишком большое число рубрик - {categoryCount.Actual}. Максимально допустимое - {categoryCount.Allowed}");
        }
    }
}