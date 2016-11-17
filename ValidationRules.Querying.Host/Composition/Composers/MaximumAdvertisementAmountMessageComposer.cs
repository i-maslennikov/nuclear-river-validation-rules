using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class MaximumAdvertisementAmountMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.MaximumAdvertisementAmount;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var dto = validationResult.ReadAdvertisementCountMessage();

            return new MessageComposerResult(
                orderReference,
                $"Позиция {dto.Name} должна присутствовать в сборке в количестве от {dto.Min} до {dto.Max}. Фактическое количество позиций в месяц {dto.Month:d} - {dto.Count}");
        }
    }
}