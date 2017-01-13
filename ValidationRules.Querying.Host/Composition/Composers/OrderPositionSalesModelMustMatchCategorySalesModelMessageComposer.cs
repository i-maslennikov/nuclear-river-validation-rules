using NuClear.ValidationRules.Querying.Host.Properties;
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
                Resources.CategoryIsRestrictedForSpecifiedSalesModelError,
                orderPositionReference,
                categoryReference,
                projectReference);
        }
    }
}