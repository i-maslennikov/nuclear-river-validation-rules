using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.ThemeRules
{
    public sealed class ThemePeriodMustContainOrderPeriodMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public ThemePeriodMustContainOrderPeriodMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.ThemePeriodMustContainOrderPeriod;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var themeReference = validationResult.ReadThemeReference();

            return new MessageTemplate(
                orderReference,
                "Заказ {0} не может иметь продаж в тематику {1}, поскольку тематика действует не весь период размещения заказа",
                _linkFactory.CreateLink(orderReference),
                _linkFactory.CreateLink(themeReference));
        }
    }
}