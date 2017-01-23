﻿using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderPositionAdvertisementMustHaveAdvertisementMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionAdvertisementMustHaveAdvertisement;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var positionReference = validationResult.ReadPositionReference();

            if (orderPositionReference.Name == positionReference.Name)
            {
                return new MessageComposerResult(
                    orderReference,
                    Resources.OrderCheckPositionMustHaveAdvertisements,
                    orderPositionReference);
            }

            return new MessageComposerResult(
                orderReference,
                Resources.OrderCheckCompositePositionMustHaveAdvertisements,
                orderPositionReference,
                positionReference);
        }
    }
}