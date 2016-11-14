using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AdvertisementCountPerCategoryShouldBeLimitedMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited;

        public MessageComposerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var categoryReference = validationResult.ReadCategoryReference();
            var dto = validationResult.ReadOversalesMessage();

            return new MessageComposerResult(
                orderReference,
                $"В рубрику {{0}} заказано слишком много объявлений: Заказано {dto.Count}, допустимо не более {dto.Max}",
                categoryReference);
        }
    }
}