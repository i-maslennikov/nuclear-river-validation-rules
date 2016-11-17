using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AssociatedPositionsGroupCountMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AssociatedPositionsGroupCount;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var price = validationResult.ReadPriceReference();
            var pricePosition = validationResult.ReadPricePositionReference();

            return new MessageComposerResult(
                price,
                "В Позиции прайс-листа {0} содержится более одной группы сопутствующих позиций, что не поддерживается системой",
                pricePosition);
        }
    }
}