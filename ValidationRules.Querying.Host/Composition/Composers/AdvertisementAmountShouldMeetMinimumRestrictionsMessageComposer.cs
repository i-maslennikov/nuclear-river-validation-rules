using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AdvertisementAmountShouldMeetMinimumRestrictionsMessageComposer : IMessageComposer, IDistinctor
    {
        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementAmountShouldMeetMinimumRestrictions;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var dto = extra.ReadAdvertisementCountMessage();

            return new MessageComposerResult(
                orderReference,
                Resources.AdvertisementAmountShouldMeetMinimumRestrictions,
                dto.Name,
                dto.Min,
                dto.Max,
                dto.Month,
                dto.Count);
        }

        public IEnumerable<Message> Distinct(IEnumerable<Message> messages)
            => messages;
    }
}