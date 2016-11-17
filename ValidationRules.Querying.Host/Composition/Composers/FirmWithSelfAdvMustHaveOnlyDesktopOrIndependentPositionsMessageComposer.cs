using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class FirmWithSelfAdvMustHaveOnlyDesktopOrIndependentPositionsMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.FirmWithSelfAdvMustHaveOnlyDesktopOrIndependentPositions;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageComposerResult(
                orderReference,
                "Позиция \"Самореклама только для ПК\" продана одновременно с рекламой в другую платформу");
        }
    }
}