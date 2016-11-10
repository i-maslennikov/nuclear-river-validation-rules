using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class ThemePeriodMustContainOrderPeriodMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.ThemePeriodMustContainOrderPeriod;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var themeReference = validationResult.ReadThemeReference();

            return new MessageSerializerResult(
                orderReference,
                "Заказ {0} не может иметь продаж в тематику {1}, поскольку тематика действует не весь период размещения заказа",
                orderReference,
                themeReference);
        }
    }
}