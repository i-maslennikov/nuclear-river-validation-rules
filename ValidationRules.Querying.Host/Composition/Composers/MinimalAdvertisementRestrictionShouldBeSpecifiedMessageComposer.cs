using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class MinimalAdvertisementRestrictionShouldBeSpecifiedMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.MinimalAdvertisementRestrictionShouldBeSpecified;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var project = references.Get<EntityTypeProject>();

            return new MessageComposerResult(
                project,
                Resources.PricePositionHasNoMinAdvertisementAmount,
                extra["name"]);
        }
    }
}