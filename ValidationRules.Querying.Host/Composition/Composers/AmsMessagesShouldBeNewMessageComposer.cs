using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AmsMessagesShouldBeNewMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AmsMessagesShouldBeNew;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            // в ERM проверка должна быть привязана либо к городу, либо к заказу
            var fakeProjectReference = new NamedReference(new Reference(EntityTypeProject.Instance.Id, 1), null);

            return new MessageComposerResult(fakeProjectReference, Resources.AmsMessagesShouldBeNew);
        }
    }
}