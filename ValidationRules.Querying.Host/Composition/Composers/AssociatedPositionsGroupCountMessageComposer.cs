using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AssociatedPositionsGroupCountMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AssociatedPositionsGroupCount;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var project = references.Get<EntityTypeProject>();
            var pricePosition = references.Get<EntityTypePricePosition>();

            return new MessageComposerResult(
                project,
                Resources.InPricePositionOf_Price_ContaiedMoreThanOneAssociatedPositions,
                pricePosition);
        }
    }
}