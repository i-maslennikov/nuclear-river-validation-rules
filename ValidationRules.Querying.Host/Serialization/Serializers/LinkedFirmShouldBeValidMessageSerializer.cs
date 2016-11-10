using System.Collections.Generic;

using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class LinkedFirmShouldBeValidMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LinkedFirmShouldBeValid;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var firmReference = validationResult.ReadFirmReference();
            var firmState = validationResult.ReadFirmState();

            var format = new Dictionary<InvalidFirmState, string>
                {
                    { InvalidFirmState.Deleted, "Фирма {0} удалена" },
                    { InvalidFirmState.ClosedForever, "Фирма {0} скрыта навсегда" },
                    { InvalidFirmState.ClosedForAscertainment, "Фирма {0} скрыта до выяснения" }
                };

            return new MessageSerializerResult(
                orderReference,
                format[firmState],
                firmReference);
        }
    }
}