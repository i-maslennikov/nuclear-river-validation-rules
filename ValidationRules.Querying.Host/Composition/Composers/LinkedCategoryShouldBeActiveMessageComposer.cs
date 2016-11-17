using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class LinkedCategoryShouldBeActiveMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LinkedCategoryShouldBeActive;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var categoryReference = validationResult.ReadCategoryReference();

            return new MessageComposerResult(
                orderReference,
                "В позиции {0} найдена неактивная рубрика {1}",
                orderPositionReference,
                categoryReference);
        }
    }
}