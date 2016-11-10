using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class AdvertisementCountPerCategoryShouldBeLimitedMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var categoryReference = validationResult.ReadCategoryReference();
            var dto = validationResult.ReadOversalesMessage();

            return new MessageSerializerResult(
                orderReference,
                $"В рубрику {{0}} заказано слишком много объявлений: Заказано {dto.Count}, допустимо не более {dto.Max}",
                categoryReference);
        }
    }
}