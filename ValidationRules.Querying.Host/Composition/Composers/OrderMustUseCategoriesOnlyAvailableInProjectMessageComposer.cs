using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderMustUseCategoriesOnlyAvailableInProjectMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderMustUseCategoriesOnlyAvailableInProject;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var categoryReference = validationResult.ReadCategoryReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();

            return new MessageComposerResult(
                orderReference,
                Resources.OrdersCheckOrderPositionContainsCategoriesFromWrongOrganizationUnit,
                categoryReference,
                orderPositionReference);
        }
    }
}