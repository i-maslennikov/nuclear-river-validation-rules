using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class SatisfiedPrincipalPositionDifferentOrderMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.SatisfiedPrincipalPositionDifferentOrder;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var principal = (OrderPositionNamedReference)references[0];
            var dependent = (OrderPositionNamedReference)references[1];

            return new MessageComposerResult(
                principal.Order,
                string.Format(
                    Resources.ADPValidation_Template,
                    principal.PositionPrefix,
                    dependent.PositionPrefix),
                principal,
                dependent,
                dependent.Order);
        }
    }
}