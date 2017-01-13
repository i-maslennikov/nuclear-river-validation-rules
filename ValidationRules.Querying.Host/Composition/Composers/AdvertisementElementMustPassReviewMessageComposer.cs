using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AdvertisementElementMustPassReviewMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementElementMustPassReview;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var advertisementReference = validationResult.ReadAdvertisementReference();
            var advertisementElementReference = validationResult.ReadAdvertisementElementReference();
            var advertisementElementStatus = validationResult.ReadAdvertisementElementStatus();

            var status = new Dictionary<Advertisement.ReviewStatus, string>
                {
                    { Advertisement.ReviewStatus.Invalid, Resources.OrdersCheckAdvertisementElementWasInvalidated},
                    { Advertisement.ReviewStatus.Draft, Resources.OrdersCheckAdvertisementElementIsDraft},
                };

            return new MessageComposerResult(
                orderReference,
                status[advertisementElementStatus],
                advertisementReference,
                advertisementElementReference);
        }
    }
}