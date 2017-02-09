using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class FirmShouldHaveLimitedCategoryCountMessageComposer : IMessageComposer, IDistinctor
    {
        public MessageTypeCode MessageType => MessageTypeCode.FirmShouldHaveLimitedCategoryCount;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var firmReference = validationResult.ReadFirmReference();
            var categoryCount = validationResult.ReadCategoryCount();

            return new MessageComposerResult(
                orderReference,
                string.Format(Resources.TooManyCategorieForFirm, categoryCount.Actual, categoryCount.Allowed),
                firmReference);
        }

        // Может быть несколько разных сообщений
        public IEnumerable<Version.ValidationResult> Distinct(IEnumerable<Version.ValidationResult> results)
            => results.GroupBy(result => result.OrderId)
                      .Select(group => group.OrderByDescending(result => result.ReadCategoryCount().Actual).First());
    }
}