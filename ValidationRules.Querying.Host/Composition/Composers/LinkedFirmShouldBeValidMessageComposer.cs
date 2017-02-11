using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class LinkedFirmShouldBeValidMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LinkedFirmShouldBeValid;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var firmReference = validationResult.ReadFirmReference();
            var firmState = validationResult.ReadFirmState();

            var format = new Dictionary<InvalidFirmState, string>
                {
                    { InvalidFirmState.Deleted, Resources.FirmIsDeleted },
                    { InvalidFirmState.ClosedForever, Resources.FirmIsPermanentlyClosed },
                    { InvalidFirmState.ClosedForAscertainment, Resources.OrderFirmHiddenForAscertainmentTemplate }
                };

            return new MessageComposerResult(
                orderReference,
                format[firmState],
                firmReference);
        }
    }
}