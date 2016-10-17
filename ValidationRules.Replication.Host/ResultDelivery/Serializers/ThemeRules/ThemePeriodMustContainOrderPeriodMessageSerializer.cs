namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ThemeRules
{
    public sealed class ThemePeriodMustContainOrderPeriodMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public ThemePeriodMustContainOrderPeriodMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.ThemePeriodMustContainOrderPeriod;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var themeReference = message.ReadThemeReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Заказ {_linkFactory.CreateLink(orderReference)} не может иметь продаж в тематику {_linkFactory.CreateLink(themeReference)}, поскольку тематика действует не весь период размещения заказа");
        }
    }
}