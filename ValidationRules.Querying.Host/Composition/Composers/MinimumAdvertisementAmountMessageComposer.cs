using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class MinimumAdvertisementAmountMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.MinimumAdvertisementAmount;

        public MessageComposerResult Compose(Message message, IReadOnlyCollection<EntityReference> references)
        {
            var orderReference = references.Get("order");
            var dto = message.ReadAdvertisementCountMessage();

            return new MessageComposerResult(
                                             orderReference,
                                             string.Format(
                                                           Resources.AdvertisementAmountShortErrorMessage,
                                                           dto.Name,
                                                           dto.Min,
                                                           dto.Max,
                                                           dto.Month,
                                                           dto.Count));
        }
    }
}