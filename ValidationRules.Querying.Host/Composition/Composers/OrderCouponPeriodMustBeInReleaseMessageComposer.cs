using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderCouponPeriodMustBeInReleaseMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderCouponPeriodMustBeInRelease;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var orderPositionReference = references.Get<EntityTypeOrderPosition>();
            var advertisementReference = references.Get<EntityTypeAdvertisement>();

            return new MessageComposerResult(
                orderReference,
                Resources.AdvertisementPeriodEndsBeforeReleasePeriodBegins,
                advertisementReference,
                orderPositionReference);
        }
    }
}