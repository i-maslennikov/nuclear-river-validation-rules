using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class PoiAmountForEntranceShouldMeetMaximumRestrictionsComposer : IMessageComposer, IDistinctor
    {
        public MessageTypeCode MessageType => MessageTypeCode.PoiAmountForEntranceShouldMeetMaximumRestrictions;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var begin = DateTime.Parse(extra["begin"]);
            var end = DateTime.Parse(extra["end"]);
            var maxCount = extra["maxCount"];
            var entranceCode = extra["entranceCode"];

            var orders = references.GetMany<EntityTypeOrder>().ToList();
            var firmAddress = references.Get<EntityTypeFirmAddress>();

            var period = begin.AddMonths(1) == end
                             ? begin.ToString("MMMM")
                             : $"{begin:MMMM} - {end:MMMM}";

            var currentOrder = orders[0];
            var conflictingOrders = orders.Skip(1).ToList();

            var conflictingOrderPlaceholders = Enumerable.Range(4, conflictingOrders.Count).Select(i => $"{{{i}}}");
            var template = Resources.PoiLimitExceededForTheEntrance.Replace("{4}", string.Join(", ", conflictingOrderPlaceholders));
            var args = new object[] { maxCount, period, firmAddress, entranceCode }.Concat(conflictingOrders).ToArray();

            return new MessageComposerResult(currentOrder,
                                             template,
                                             args);
        }

        public IEnumerable<Message> Distinct(IEnumerable<Message> messages)
        {
            return messages.GroupBy(x => new
                                        {
                                            x.OrderId,
                                            x.MessageType,
                                            x.ProjectId,
                                            Begin = x.Extra["begin"],
                                            End = x.Extra["end"],
                                            MaxCount = x.Extra["maxCount"],
                                            EntranceCode = x.Extra["entranceCode"],
                                            FirmAddressId = x.References.Get<EntityTypeFirmAddress>().Id
                                        },
                                    x => x.References)
                           .Select(x => new Message
                               {
                                   OrderId = x.Key.OrderId,
                                   MessageType = x.Key.MessageType,
                                   ProjectId = x.Key.ProjectId,
                                   Extra = new Dictionary<string, string>
                                       {
                                           ["begin"] = x.Key.Begin,
                                           ["end"] = x.Key.End,
                                           ["maxCount"] = x.Key.MaxCount,
                                           ["entranceCode"] = x.Key.EntranceCode
                                       },
                                   References = new[] { new Reference<EntityTypeOrder>(x.Key.OrderId.Value) }.Concat(x.SelectMany(y => y)).ToList()
                               });
        }
    }
}