using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
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

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var orderPositionReference = references.Get<EntityTypeOrderPositionAdvertisement>();
            var firmAddressReference = references.Get<EntityTypeFirmAddress>();

            var firmAddressState = extra.ReadFirmAddressState();

            return new MessageComposerResult(
                orderReference,
                Formats[firmAddressState],
                orderPositionReference,
                firmAddressReference);
        }
    }
}