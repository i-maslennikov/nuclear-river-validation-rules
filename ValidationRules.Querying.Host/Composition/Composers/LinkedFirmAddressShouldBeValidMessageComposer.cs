using System.Collections.Generic;

using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

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
                    { InvalidFirmAddressState.Deleted, "скрыт навсегда" },
                    { InvalidFirmAddressState.NotActive, "неактивен" },
                    { InvalidFirmAddressState.ClosedForAscertainment, "скрыт до выяснения" },
                    { InvalidFirmAddressState.NotBelongToFirm, "не принадлежит фирме заказа" }
                };

            return new MessageComposerResult(
                orderReference,
                $"В позиции {{0}} адрес фирмы {{1}} {format[firmAddressState]}",
                orderPositionReference,
                firmAddressReference);
        }
    }
}