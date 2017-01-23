using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class LegalPersonProfileWarrantyShouldNotBeExpiredMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LegalPersonProfileWarrantyShouldNotBeExpired;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var legalPersonProfileReference = validationResult.ReadLegalPersonProfileReference();

            return new MessageComposerResult(
                orderReference,
                Resources.ProfileWarrantyEndDateIsLessThanSignDate,
                legalPersonProfileReference);
        }
    }
}