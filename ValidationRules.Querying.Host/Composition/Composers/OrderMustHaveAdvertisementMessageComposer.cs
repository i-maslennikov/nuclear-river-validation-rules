using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderMustHaveAdvertisementMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderMustHaveAdvertisement;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var advertisementReference = validationResult.ReadAdvertisementReference();
            var advertisementElementReference = validationResult.ReadAdvertisementElementReference();

            return new MessageComposerResult(
                orderReference,
                "В рекламном материале {0} не заполнен обязательный элемент {1}",
                advertisementReference,
                advertisementElementReference);
        }
    }
}