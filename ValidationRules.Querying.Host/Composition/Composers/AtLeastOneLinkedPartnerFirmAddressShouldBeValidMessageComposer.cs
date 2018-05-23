using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public class AtLeastOneLinkedPartnerFirmAddressShouldBeValidMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType { get; } = MessageTypeCode.AtLeastOneLinkedPartnerFirmAddressShouldBeValid;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderPosition = (OrderPositionNamedReference)references.GetMany<EntityTypeOrderPosition>().First();

            return new MessageComposerResult(orderPosition.Order,
                                             Resources.AtLeastOneLinkedPartnerFirmAddressShouldBeValid,
                                             orderPosition);
        }
    }
}