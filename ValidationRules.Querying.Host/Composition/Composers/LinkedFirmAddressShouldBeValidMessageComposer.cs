using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class LinkedFirmAddressShouldBeValidMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LinkedFirmAddressShouldBeValid;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var firmAddressReference = validationResult.ReadFirmAddressReference();

            var firmAddressState = validationResult.ReadFirmAddressState();
            var format = new Dictionary<InvalidFirmAddressState, string>
                {
                    { InvalidFirmAddressState.Deleted, Resources.OrderPositionAddressDeleted },
                    { InvalidFirmAddressState.NotActive, Resources.OrderPositionAddressNotActive },
                    { InvalidFirmAddressState.ClosedForAscertainment, Resources.OrderPositionAddressHidden },
                    { InvalidFirmAddressState.NotBelongToFirm, Resources.OrderPositionAddressNotBelongToFirm }
                };

            return new MessageComposerResult(
                orderReference,
                format[firmAddressState],
                orderPositionReference,
                firmAddressReference);
        }
    }
}