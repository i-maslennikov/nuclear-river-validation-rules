namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.PriceRules
{
    public sealed class AdvertisementCountPerThemeShouldBeLimitedMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public AdvertisementCountPerThemeShouldBeLimitedMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementCountPerThemeShouldBeLimited;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var themeReference = message.ReadThemeReference();
            var dto = message.ReadOversalesMessage();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Слишком много продаж в тематику {_linkFactory.CreateLink(themeReference)}. Продано {dto.Count} позиций вместо {dto.Max} возможных");
        }
    }
}