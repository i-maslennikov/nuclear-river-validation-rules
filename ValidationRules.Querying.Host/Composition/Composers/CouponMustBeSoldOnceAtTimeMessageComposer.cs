using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class CouponMustBeSoldOnceAtTimeMessageComposer : IMessageComposer, IDistinctor
    {
        public MessageTypeCode MessageType => MessageTypeCode.CouponMustBeSoldOnceAtTime;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var advertisementReference = references.Get<EntityTypeAdvertisement>();
            // todo: сортировки нет в требованиях, сделана для соответствия erm
            var orderPositionReferences = references.GetMany<EntityTypeOrderPosition>().OrderBy(x => x.Reference.Id);

            return new MessageComposerResult(
                orderReference,
                string.Format(Resources.CouponIsBoundToMultiplePositionTemplate, string.Join(", ", orderPositionReferences.Select((x, i) => "{" + (i + 1) + "}"))),
                new [] { advertisementReference }.Concat(orderPositionReferences).ToArray());
        }

        public IEnumerable<Message> Distinct(IEnumerable<Message> messages)
            => messages.GroupBy(x => new { x.OrderId, x.MessageType, x.ProjectId, AdvertisementId = x.References.Get<EntityTypeAdvertisement>().Id }, x => x.References)
                       .Select(x => new Message
                           {
                               OrderId = x.Key.OrderId,
                               MessageType = x.Key.MessageType,
                               ProjectId = x.Key.ProjectId,
                               References = new List<Reference>(x.SelectMany(ReferenceExtensions.GetMany<EntityTypeOrderPositionAdvertisement>))
                                {
                                    new Reference(EntityTypeOrder.Instance.Id, x.Key.OrderId.Value),
                                    new Reference(EntityTypeAdvertisement.Instance.Id, x.Key.AdvertisementId),
                                },
                           });
    }
}