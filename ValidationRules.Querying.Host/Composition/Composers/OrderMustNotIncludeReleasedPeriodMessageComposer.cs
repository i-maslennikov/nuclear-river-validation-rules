using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderMustNotIncludeReleasedPeriodMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderMustNotIncludeReleasedPeriod;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();

            return new MessageComposerResult(
                orderReference,
                Resources.OrderCheckHasReleases);
        }
    }
}