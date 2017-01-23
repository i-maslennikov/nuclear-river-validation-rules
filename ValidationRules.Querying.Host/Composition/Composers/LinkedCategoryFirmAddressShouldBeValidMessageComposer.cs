using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class LinkedCategoryFirmAddressShouldBeValidMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LinkedCategoryFirmAddressShouldBeValid;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var categoryReference = validationResult.ReadCategoryReference();
            var firmAddressReference = validationResult.ReadFirmAddressReference();

            return new MessageComposerResult(
                orderReference,
                Resources.OrderPositionCategoryNotBelongsToAddress,
                orderPositionReference,
                categoryReference,
                firmAddressReference);
        }
    }
}