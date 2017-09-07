using System;
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

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var advertisementReference = references.Get<EntityTypeAdvertisement>();
            var advertisementElementReference = references.Get<EntityTypeAdvertisementElement>();
            var advertisementElementStatus = extra.ReadAdvertisementElementStatus();

            return new MessageComposerResult(
                orderReference,
                GetFormat(advertisementElementStatus),
                advertisementReference,
                advertisementElementReference);
        }

        private static string GetFormat(Advertisement.ReviewStatus reviewStatus)
        {
            switch (reviewStatus)
            {
                case Advertisement.ReviewStatus.Invalid:
                    return Resources.OrdersCheckAdvertisementElementWasInvalidated;
                case Advertisement.ReviewStatus.Draft:
                    return Resources.OrdersCheckAdvertisementElementIsDraft;
                default:
                    throw new ArgumentException(nameof(reviewStatus));
            }
        }
    }
}