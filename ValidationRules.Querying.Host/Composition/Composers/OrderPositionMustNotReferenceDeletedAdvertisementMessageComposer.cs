using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderPositionMustNotReferenceDeletedAdvertisementMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionMustNotReferenceDeletedAdvertisement;

        public MessageComposerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var advertisementReference = validationResult.ReadAdvertisementReference();

            return new MessageComposerResult(
                orderReference,
                "В позиции {0} выбран удалённый рекламный материал {1}",
                orderPositionReference,
                advertisementReference);
        }
    }
}