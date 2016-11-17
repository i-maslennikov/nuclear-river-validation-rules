using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class FirmAddressMustBeLocatedOnTheMapMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.FirmAddressMustBeLocatedOnTheMap;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var firmAddressReference = validationResult.ReadFirmAddressReference();

            return new MessageComposerResult(
                orderReference,
                "Позиция {0} оформлена на пустой адрес {1}",
                orderPositionReference,
                firmAddressReference);
        }
    }
}