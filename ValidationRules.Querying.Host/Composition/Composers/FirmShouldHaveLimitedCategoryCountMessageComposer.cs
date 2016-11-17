using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class FirmShouldHaveLimitedCategoryCountMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.FirmShouldHaveLimitedCategoryCount;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var firmReference = validationResult.ReadFirmReference();
            var categoryCount = validationResult.ReadCategoryCount();

            return new MessageComposerResult(
                orderReference,
                $"Для фирмы {{0}} задано слишком большое число рубрик - {categoryCount.Actual}. Максимально допустимое - {categoryCount.Allowed}",
                firmReference);
        }
    }
}