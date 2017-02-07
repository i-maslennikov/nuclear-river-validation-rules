using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class LinkedFirmAddressShouldBeValidMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LinkedFirmAddressShouldBeValid;

        private static readonly Dictionary<InvalidFirmAddressState, string> Formats = new Dictionary<InvalidFirmAddressState, string>
        {
            { InvalidFirmAddressState.Deleted, Resources.OrderPositionAddressDeleted },
            { InvalidFirmAddressState.NotActive, Resources.OrderPositionAddressNotActive },
            { InvalidFirmAddressState.ClosedForAscertainment, Resources.OrderPositionAddressHidden },
            { InvalidFirmAddressState.NotBelongToFirm, Resources.OrderPositionAddressNotBelongToFirm }
        };

        public MessageComposerResult Compose(Message message, IReadOnlyCollection<EntityReference> references)
        {
            var orderReference = references.Get("order");
            var orderPositionReference = references.Get("orderPosition");
            var firmAddressReference = references.Get("firmAddress");

            var firmAddressState = message.ReadFirmAddressState();

            return new MessageComposerResult(
                orderReference,
                Formats[firmAddressState],
                orderPositionReference,
                firmAddressReference);
        }
    }
}