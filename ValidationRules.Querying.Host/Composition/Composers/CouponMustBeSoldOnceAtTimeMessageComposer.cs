using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class CouponMustBeSoldOnceAtTimeMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.CouponMustBeSoldOnceAtTime;

        public MessageComposerResult Compose(Message message, IReadOnlyCollection<EntityReference> references)
        {
            var orderReference = references.Get("order");
            var advertisementReference = references.Get("advertisement");
            var orderPositionReferences = references.GetMany("orderPosition").ToList();

            var referencePlaceholders = orderPositionReferences.Select((x, i) => "{" + (i + 1) + "}");

            return new MessageComposerResult(
                orderReference,
                string.Format(Resources.CouponIsBoundToMultiplePositionTemplate, string.Join(", ", referencePlaceholders)),
                new [] { advertisementReference }.Concat(orderPositionReferences.OrderBy(x => x.Id)).ToArray()); // todo: сортировки нет в требованиях, сделана для соответствия erm
        }
    }
}