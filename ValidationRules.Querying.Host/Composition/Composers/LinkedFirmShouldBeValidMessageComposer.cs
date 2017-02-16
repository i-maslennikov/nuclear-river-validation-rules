using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class LinkedFirmShouldBeValidMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LinkedFirmShouldBeValid;

        private static readonly Dictionary<InvalidFirmState, string> Formats = new Dictionary<InvalidFirmState, string>
        {
            { InvalidFirmState.Deleted, Resources.FirmIsDeleted },
            { InvalidFirmState.ClosedForever, Resources.FirmIsPermanentlyClosed },
            { InvalidFirmState.ClosedForAscertainment, Resources.OrderFirmHiddenForAscertainmentTemplate }
        };

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var firmReference = references.Get<EntityTypeFirm>();
            var firmState = extra.ReadFirmState();

            return new MessageComposerResult(
                orderReference,
                Formats[firmState],
                firmReference);
        }
    }
}