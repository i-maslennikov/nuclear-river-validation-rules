using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AdvertisementAmountShouldMeetRestrictionsMassMessageComposer : IMessageComposer, IDistinctor
    {
        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementAmountShouldMeetMinimumRestrictionsMass;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var projectReference = references.Get<EntityTypeProject>();
            var dto = extra.ReadAdvertisementCountMessage();

            return new MessageComposerResult(
                projectReference,
                string.Format(
                    Resources.AdvertisementAmountShortErrorMessage,
                    dto.Name.ClearBrackets(),
                    dto.Min,
                    dto.Max,
                    dto.Month,
                    dto.Count));
        }

        public IEnumerable<Message> Distinct(IEnumerable<Message> messages)
            => messages;
    }
}