using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderPositionCostPerClickMustNotBeLessMinimumMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionCostPerClickMustNotBeLessMinimum;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var categoryReference = validationResult.ReadCategoryReference();

            return new MessageComposerResult(
                orderReference,
                "Для позиции {0} в рубрику {1} указан CPC меньше минимального",
                orderPositionReference,
                categoryReference);
        }
    }
}