using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class FirmShouldHaveLimitedCategoryCountMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.FirmShouldHaveLimitedCategoryCount;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var firmReference = validationResult.ReadFirmReference();
            var categoryCount = validationResult.ReadCategoryCount();

            return new MessageSerializerResult(
                orderReference,
                $"Для фирмы {{0}} задано слишком большое число рубрик - {categoryCount.Actual}. Максимально допустимое - {categoryCount.Allowed}",
                firmReference);
        }
    }
}