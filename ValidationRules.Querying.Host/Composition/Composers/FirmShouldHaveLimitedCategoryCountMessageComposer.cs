using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class FirmShouldHaveLimitedCategoryCountMessageComposer : IMessageComposer, IDistinctor
    {
        public MessageTypeCode MessageType => MessageTypeCode.FirmShouldHaveLimitedCategoryCount;

        public MessageComposerResult Compose(Message message, IReadOnlyCollection<EntityReference> references)
        {
            var orderReference = references.Get("order");
            var firmReference = references.Get("firm");
            var categoryCount = message.ReadCategoryCount();

            return new MessageComposerResult(
                orderReference,
                string.Format(Resources.TooManyCategorieForFirm, categoryCount.Actual, categoryCount.Allowed),
                firmReference);
        }

        // Может быть несколько разных сообщений
        public IEnumerable<Message> Distinct(IEnumerable<Message> messages)
            => messages.GroupBy(result => result.OrderId)
                      .Select(group => group.OrderByDescending(result => result.ReadCategoryCount().Actual).First());
    }
}