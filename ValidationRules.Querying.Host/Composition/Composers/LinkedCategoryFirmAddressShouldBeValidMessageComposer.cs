using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class LinkedCategoryFirmAddressShouldBeValidMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LinkedCategoryFirmAddressShouldBeValid;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var orderPositionReference = references.Get<EntityTypeOrderPositionAdvertisement>();
            var categoryReference = references.Get<EntityTypeCategory>();
            var firmAddressReference = references.Get<EntityTypeFirmAddress>();

            return new MessageComposerResult(
                orderReference,
                Resources.OrderPositionCategoryNotBelongsToAddress,
                orderPositionReference,
                categoryReference,
                firmAddressReference);
        }
    }
}