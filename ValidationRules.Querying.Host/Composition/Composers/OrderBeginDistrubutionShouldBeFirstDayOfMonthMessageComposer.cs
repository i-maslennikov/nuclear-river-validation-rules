using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderBeginDistrubutionShouldBeFirstDayOfMonthMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderBeginDistrubutionShouldBeFirstDayOfMonth;

        public MessageComposerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageComposerResult(
                orderReference,
                "Указана некорректная дата начала размещения");
        }
    }
}