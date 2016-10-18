namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ThemeRules
{
    public sealed class DefaultThemeMustHaveOnlySelfAdsMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public DefaultThemeMustHaveOnlySelfAdsMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.DefaultThemeMustHaveOnlySelfAds;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var themeReference = message.ReadThemeReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Установленная по умолчанию тематика {_linkFactory.CreateLink(themeReference)} должна содержать только саморекламу");
        }
    }
}