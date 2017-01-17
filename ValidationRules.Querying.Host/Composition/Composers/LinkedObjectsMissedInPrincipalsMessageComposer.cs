using System.Linq;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class LinkedObjectsMissedInPrincipalsMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LinkedObjectsMissedInPrincipals;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositions = validationResult.ReadOrderPositions();

            var first = orderPositions.First();

            return new MessageComposerResult(
                orderReference,
                string.Format(Resources.LinkedObjectsMissedInPrincipals, MakePositionText(first)),
                new EntityReference("OrderPosition", first.OrderPositionId, first.OrderPositionName));
        }

        private static string MakePositionText(ResultExtensions.OrderPositionDto dto)
        {
            return dto.OrderPositionName != dto.PositionName
                       ? string.Format(Resources.RichChildPositionTypeTemplate, dto.PositionName)
                       : Resources.RichDefaultPositionTypeTemplate;
        }
    }
}