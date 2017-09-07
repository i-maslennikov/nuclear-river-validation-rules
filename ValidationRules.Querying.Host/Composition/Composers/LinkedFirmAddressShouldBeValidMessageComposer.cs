using System;
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

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var orderPositionReference = references.Get<EntityTypeOrderPosition>();
            var firmAddressReference = references.Get<EntityTypeFirmAddress>();

            var firmAddressState = extra.ReadFirmAddressState();

            return new MessageComposerResult(
                orderReference,
                GetFormat(firmAddressState),
                orderPositionReference,
                firmAddressReference);
        }

        private static string GetFormat(InvalidFirmAddressState firmAddressState)
        {
            switch (firmAddressState)
            {
                case InvalidFirmAddressState.Deleted:
                    return Resources.OrderPositionAddressDeleted;
                case InvalidFirmAddressState.NotActive:
                    return Resources.OrderPositionAddressNotActive;
                case InvalidFirmAddressState.ClosedForAscertainment:
                    return Resources.OrderPositionAddressHidden;
                case InvalidFirmAddressState.NotBelongToFirm:
                    return Resources.OrderPositionAddressNotBelongToFirm;
                default:
                    throw new Exception(nameof(firmAddressState));
            }
        }
    }
}