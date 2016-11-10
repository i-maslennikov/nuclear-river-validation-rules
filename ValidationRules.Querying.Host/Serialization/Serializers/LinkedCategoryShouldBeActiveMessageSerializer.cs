using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class LinkedCategoryShouldBeActiveMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LinkedCategoryShouldBeActive;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var categoryReference = validationResult.ReadCategoryReference();

            return new MessageSerializerResult(
                orderReference,
                "В позиции {0} найдена неактивная рубрика {1}",
                orderPositionReference,
                categoryReference);
        }
    }
}