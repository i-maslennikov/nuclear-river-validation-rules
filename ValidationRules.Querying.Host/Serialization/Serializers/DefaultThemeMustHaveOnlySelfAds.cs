using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class DefaultThemeMustHaveOnlySelfAdsMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.DefaultThemeMustHaveOnlySelfAds;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var themeReference = validationResult.ReadThemeReference();

            return new MessageSerializerResult(
                orderReference,
                "Установленная по умолчанию тематика {0} должна содержать только саморекламу",
                themeReference);
        }
    }
}