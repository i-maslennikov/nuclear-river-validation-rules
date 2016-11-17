using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AdvertisementCountPerThemeShouldBeLimitedMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementCountPerThemeShouldBeLimited;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var themeReference = validationResult.ReadThemeReference();
            var dto = validationResult.ReadOversalesMessage();

            return new MessageComposerResult(
                orderReference,
                $"Слишком много продаж в тематику {{0}}. Продано {dto.Count} позиций вместо {dto.Max} возможных",
                themeReference);
        }
    }
}