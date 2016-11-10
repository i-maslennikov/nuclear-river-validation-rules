using System.Linq;

using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class CouponMustBeSoldOnceAtTimeMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.CouponMustBeSoldOnceAtTime;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var advertisementReference = validationResult.ReadAdvertisementReference();
            var orderPositionReferences = validationResult.ReadOrderPositionReferences();

            var referencePlaceholders = orderPositionReferences.Select((x, i) => "{" + (i + 1) + "}");

            return new MessageSerializerResult(
                orderReference,
                $"Купон на скидку {{0}} прикреплён к нескольким позициям: {string.Join(", ", referencePlaceholders)}",
                new [] { advertisementReference }.Concat(orderPositionReferences).ToArray());
        }
    }
}