using System;
using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OptionalAdvertisementMustPassReviewMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OptionalAdvertisementMustPassReview;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var advertisementReference = references.Get<EntityTypeAdvertisement>();
            var advertisementElementState = extra.ReadAdvertisementReviewState();

            return new MessageComposerResult(
                                             orderReference,
                                             GetTemplate(advertisementElementState),
                                             advertisementReference);
        }

        private string GetTemplate(Order.AdvertisementReviewState state)
        {
            switch (state)
            {
                case Order.AdvertisementReviewState.Draft:
                    return Resources.OptionalAdvertisementMustPassReview_Draft;
                case Order.AdvertisementReviewState.Invalid:
                    return Resources.OptionalAdvertisementMustPassReview_Invalid;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}