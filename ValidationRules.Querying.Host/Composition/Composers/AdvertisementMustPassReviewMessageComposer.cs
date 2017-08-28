using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AdvertisementMustPassReviewMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementMustPassReview;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var advertisementReference = references.Get<EntityTypeAdvertisement>();

            // todo: для параметра advertisementElementState нужны описания кодов ошибок, пока выводится общее сообщение
            var advertisementElementState = extra.ReadAdvertisementReviewState();

            return new MessageComposerResult(
                orderReference,
                Resources.AdvertisementMustPassReview,
                advertisementReference);
        }
    }
}