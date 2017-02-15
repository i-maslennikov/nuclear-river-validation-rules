using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class LinkedCategoryShouldBelongToFirmMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LinkedCategoryShouldBelongToFirm;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var orderPositionReference = references.Get<EntityTypeOrderPosition>();
            var categoryReference = references.Get<EntityTypeCategory>();

            return new MessageComposerResult(
                orderReference,
                Resources.OrderPositionCategoryNotBelongsToFirm,
                orderPositionReference,
                categoryReference);
        }
    }
}