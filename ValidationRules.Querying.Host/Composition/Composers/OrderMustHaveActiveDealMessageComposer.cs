using System.Collections.Generic;

using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderMustHaveActiveDealMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderMustHaveActiveDeal;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var dealState = validationResult.ReadDealState();

            var format = new Dictionary<ResultExtensions.DealState, string>
            {
                { ResultExtensions.DealState.Missing, "Для заказа не указана работа" },
                { ResultExtensions.DealState.Inactive, "Для заказа указана неактивная работа" }
            };

            return new MessageComposerResult(orderReference, format[dealState]);
        }
    }
}