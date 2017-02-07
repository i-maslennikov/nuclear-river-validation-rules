using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AdvertisementElementMustPassReviewMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementElementMustPassReview;

        private static readonly Dictionary<Advertisement.ReviewStatus, string> Formats = new Dictionary<Advertisement.ReviewStatus, string>
        {
            { Advertisement.ReviewStatus.Invalid, Resources.OrdersCheckAdvertisementElementWasInvalidated},
            { Advertisement.ReviewStatus.Draft, Resources.OrdersCheckAdvertisementElementIsDraft},
        };

        public MessageComposerResult Compose(Message message, IReadOnlyCollection<EntityReference> references)
        {
            var orderReference = references.Get("order");
            var advertisementReference = references.Get("advertisement");
            var advertisementElementReference = references.Get("advertisementElement");
            var advertisementElementStatus = message.ReadAdvertisementElementStatus();

            return new MessageComposerResult(
                orderReference,
                Formats[advertisementElementStatus],
                advertisementReference,
                advertisementElementReference);
        }
    }
}