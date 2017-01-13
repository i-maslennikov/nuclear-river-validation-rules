using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class MinimumAdvertisementAmountMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.MinimumAdvertisementAmount;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var dto = validationResult.ReadAdvertisementCountMessage();

            return new MessageComposerResult(
                                             orderReference,
                                             string.Format(
                                                           Resources.AdvertisementAmountShortErrorMessage,
                                                           dto.Name,
                                                           dto.Min,
                                                           dto.Max,
                                                           dto.Month,
                                                           dto.Count));
        }
    }
}