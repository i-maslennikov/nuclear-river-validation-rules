using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.Composition.Composers.Utils;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class FirmAddressMustNotHaveMultipleCallToActionMessageComposer : IMessageComposer, IDistinctor
    {
        public MessageTypeCode MessageType => MessageTypeCode.FirmAddressMustNotHaveMultipleCallToAction;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var firmReference = references.Get<EntityTypeFirm>();
            var addressReference = references.Get<EntityTypeFirmAddress>();
            var periods = extra.ExtractPeriods();

            return new MessageComposerResult(
                orderReference,
                Resources.FirmAddressMustNotHaveMultipleCallToAction,
                addressReference,
                firmReference,
                string.Join(", ", periods.OrderBy(x => x).Select(x => x.ToString("MMMM yyyy"))));
        }

        public IEnumerable<Message> Distinct(IEnumerable<Message> messages)
            => messages
                .GroupBy(x => new { x.OrderId, FirmAddressId = x.References.Get<EntityTypeFirmAddress>().Id, FirmId = x.References.Get<EntityTypeFirm>().Id },
                         x => x)
                .Select(group => Merge(group.Key.OrderId.Value, group.ToList()));

        private static Message Merge(long orderId, IReadOnlyCollection<Message> messages)
        {
            var any = messages.First();

            return new Message
            {
                OrderId = orderId,
                MessageType = any.MessageType,
                ProjectId = any.ProjectId,
                References = any.References,
                Extra = messages.StorePeriods(new Dictionary<string, string>())
            };
        }
    }
}