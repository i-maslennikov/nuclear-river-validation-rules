using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderMustHaveActiveDealMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderMustHaveActiveDeal;

        private static readonly Dictionary<ResultExtensions.DealState, string> Formats = new Dictionary<ResultExtensions.DealState, string>
        {
            { ResultExtensions.DealState.Missing, Resources.ThereIsNoSpecifiedDealForOrder },
            { ResultExtensions.DealState.Inactive, Resources.OrderDealIsInactive }
        };

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var dealState = extra.ReadDealState();

            return new MessageComposerResult(
                orderReference,
                Formats[dealState]);
        }
    }
}