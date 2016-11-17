using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderPositionSalesModelMustMatchCategorySalesModelMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionSalesModelMustMatchCategorySalesModel;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var categoryReference = validationResult.ReadCategoryReference();
            var projectReference = validationResult.ReadProjectReference();

            return new MessageComposerResult(
                orderReference,
                "Позиция \"{0}\" не может быть продана в рубрику \"{1}\" проекта \"{2}\"",
                orderPositionReference,
                categoryReference,
                projectReference);
        }
    }
}