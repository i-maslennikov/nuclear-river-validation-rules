using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AdvertisementCountPerCategoryShouldBeLimitedMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var categoryReference = validationResult.ReadCategoryReference();
            var dto = validationResult.ReadOversalesMessage();

            return new MessageComposerResult(
                orderReference,
                string.Format(Resources.TooManyAdvertisementForCategory, dto.Count, dto.Max),
                categoryReference);
        }
    }
}