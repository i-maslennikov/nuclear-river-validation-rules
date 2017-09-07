using System;
using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderMustHaveActiveDealMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderMustHaveActiveDeal;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var dealState = extra.ReadDealState();

            return new MessageComposerResult(
                orderReference,
                GetFormat(dealState));
        }

        private static string GetFormat(DealState dealState)
        {
            switch (dealState)
            {
                case DealState.Missing:
                    return Resources.ThereIsNoSpecifiedDealForOrder;
                case DealState.Inactive:
                    return Resources.OrderDealIsInactive;
                default:
                    throw new Exception(nameof(dealState));
            }
        }
    }
}