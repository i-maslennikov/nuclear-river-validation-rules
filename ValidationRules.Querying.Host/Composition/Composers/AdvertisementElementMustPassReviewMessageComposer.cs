using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
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

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var advertisementReference = references.Get<EntityTypeAdvertisement>();
            var advertisementElementReference = references.Get<EntityTypeAdvertisementElement>();
            var advertisementElementStatus = extra.ReadAdvertisementElementStatus();

            return new MessageComposerResult(
                orderReference,
                Formats[advertisementElementStatus],
                advertisementReference,
                advertisementElementReference);
        }
    }
}