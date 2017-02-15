using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class LinkedObjectsMissedInPrincipalsMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LinkedObjectsMissedInPrincipals;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var orderPositionReference = (OrderPositionNamedReference)references.Get<EntityTypeOrderPosition>();

            return new MessageComposerResult(
                orderReference,
                string.Format(Resources.LinkedObjectsMissedInPrincipals, orderPositionReference.PositionPrefix),
                orderPositionReference);
        }
    }
}