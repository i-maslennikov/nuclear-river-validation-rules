using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AdvertisementWebsiteShouldNotBeFirmWebsiteMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementWebsiteShouldNotBeFirmWebsite;

        public MessageComposerResult Compose(Message message, IReadOnlyCollection<EntityReference> references)
        {
            var orderReference = references.Get("order");
            var firmReference = references.Get("firm");
            var orderPositionReference = references.Get("orderPosition");
            var website = message.ReadWebsite();

            return new MessageComposerResult(
                orderReference,
                string.Format(Resources.FirmContactContainsSponsoredLinkError, website),
                firmReference,
                orderPositionReference,
                orderReference);
        }
    }
}