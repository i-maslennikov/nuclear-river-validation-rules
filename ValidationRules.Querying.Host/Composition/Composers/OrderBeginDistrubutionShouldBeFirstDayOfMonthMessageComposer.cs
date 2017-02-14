using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderBeginDistrubutionShouldBeFirstDayOfMonthMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderBeginDistrubutionShouldBeFirstDayOfMonth;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();

            return new MessageComposerResult(
                orderReference,
                Resources.OrderCheckIncorrectBeginDistributionDate);
        }
    }
}