using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class AdvertisementCountPerThemeShouldBeLimitedMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementCountPerThemeShouldBeLimited;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var themeReference = validationResult.ReadThemeReference();
            var dto = validationResult.ReadOversalesMessage();

            return new MessageSerializerResult(
                orderReference,
                $"Слишком много продаж в тематику {{0}}. Продано {dto.Count} позиций вместо {dto.Max} возможных",
                themeReference);
        }
    }
}