using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class DeniedPositionsCheckMessageComposer : IMessageComposer, IDistinctor
    {
        public MessageTypeCode MessageType => MessageTypeCode.FirmPositionMustNotHaveDeniedPositions;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var dependent = (OrderPositionNamedReference)references[0];
            var principal = (OrderPositionNamedReference)references[1];

            if (dependent.Order.Reference.Id != principal.Order.Reference.Id)
            {
                return new MessageComposerResult(
                    dependent.Order,
                    Resources.FirmPositionMustNotHaveDeniedPositions_Order,
                    dependent.PositionPrefix,
                    dependent,
                    principal.PositionPrefix,
                    principal,
                    principal.Order);
            }
            else
            {
                return new MessageComposerResult(
                    dependent.Order,
                    Resources.FirmPositionMustNotHaveDeniedPositions,
                    dependent.PositionPrefix,
                    dependent,
                    principal.PositionPrefix,
                    principal);
            }
        }

        public IEnumerable<Message> Distinct(IEnumerable<Message> messages)
            => messages.Distinct(RuleMessageEqualityComparer.Instance);

        internal sealed class RuleMessageEqualityComparer : IEqualityComparer<Message>
        {
            public static readonly IEqualityComparer<Message> Instance = new RuleMessageEqualityComparer();

            public bool Equals(Message x, Message y)
            {
                if (x.OrderId != y.OrderId)
                {
                    return false;
                }

                var uniqueReferenceCount = x.References.Concat(y.References).Distinct(Reference.Comparer).Count();
                return uniqueReferenceCount == x.References.Count;
            }

            public int GetHashCode(Message obj)
            {
                // должен позволять перестановку ссылок в сообщении, т.е. (a, b, c) == (c, a, b)
                return obj.References.Aggregate(0, (code, reference) => code ^ Reference.Comparer.GetHashCode(reference));
            }
        }
    }
}