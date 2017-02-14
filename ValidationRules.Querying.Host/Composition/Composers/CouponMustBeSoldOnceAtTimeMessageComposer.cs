using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class CouponMustBeSoldOnceAtTimeMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.CouponMustBeSoldOnceAtTime;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var advertisementReference = references.Get<EntityTypeAdvertisement>();
            var orderPositionReferences = references.GetMany<EntityTypeOrderPosition>();

            var referencePlaceholders = orderPositionReferences.Select((x, i) => "{" + (i + 1) + "}");

            return new MessageComposerResult(
                orderReference,
                string.Format(Resources.CouponIsBoundToMultiplePositionTemplate, string.Join(", ", referencePlaceholders)),
                new [] { advertisementReference }.Concat(orderPositionReferences.OrderBy(x => x.Id)).ToArray()); // todo: сортировки нет в требованиях, сделана для соответствия erm
        }
    }
}