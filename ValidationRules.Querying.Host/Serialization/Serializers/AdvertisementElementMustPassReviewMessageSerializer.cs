using System.Collections.Generic;

using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class AdvertisementElementMustPassReviewMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementElementMustPassReview;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var advertisementReference = validationResult.ReadAdvertisementReference();
            var advertisementElementReference = validationResult.ReadAdvertisementElementReference();
            var advertisementElementStatus = validationResult.ReadAdvertisementElementStatus();

            var status = new Dictionary<Advertisement.ReviewStatus, string>
                {
                    { Advertisement.ReviewStatus.Invalid, "содержит ошибки выверки"},
                    { Advertisement.ReviewStatus.Draft, "находится в статусе 'Черновик'"},
                };

            return new MessageSerializerResult(
                orderReference,
                $"В рекламном материале \"{{0}}\", который подлежит выверке, элемент \"{{1}}\" {status[advertisementElementStatus]}",
                advertisementReference,
                advertisementElementReference);
        }
    }
}