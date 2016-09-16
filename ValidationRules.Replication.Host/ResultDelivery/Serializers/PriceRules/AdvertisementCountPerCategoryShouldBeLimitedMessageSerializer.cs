namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.PriceRules
{
    public sealed class AdvertisementCountPerCategoryShouldBeLimitedMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public AdvertisementCountPerCategoryShouldBeLimitedMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var categoryReference = message.ReadCategoryReference();
            var dto = message.ReadOversalesMessage();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"В рубрику {_linkFactory.CreateLink(categoryReference)} заказано слишком много объявлений: Заказано {dto.Count}, допустимо не более {dto.Max}.");
        }
    }
}