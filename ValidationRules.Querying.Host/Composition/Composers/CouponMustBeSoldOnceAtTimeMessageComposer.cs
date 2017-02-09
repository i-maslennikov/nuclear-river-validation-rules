using System.Linq;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class CouponMustBeSoldOnceAtTimeMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.CouponMustBeSoldOnceAtTime;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var advertisementReference = validationResult.ReadAdvertisementReference();
            var orderPositionReferences = validationResult.ReadOrderPositionReferences();

            var referencePlaceholders = orderPositionReferences.Select((x, i) => "{" + (i + 1) + "}");

            return new MessageComposerResult(
                orderReference,
                string.Format(Resources.CouponIsBoundToMultiplePositionTemplate, string.Join(", ", referencePlaceholders)),
                new [] { advertisementReference }.Concat(orderPositionReferences.OrderBy(x => x.Id)).ToArray()); // todo: сортировки нет в требованиях, сделана для соответствия erm
        }
    }
}