using System.Linq;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class DeniedPositionsCheckMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.DeniedPositionsCheck;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var positions = validationResult.ReadOrderPositions().OrderBy(x => x.OrderPositionId);
            var first = positions.First();
            var second = positions.Last();

            return new MessageComposerResult(
                orderReference,
                string.Format(Resources.ADPCheckModeSpecificOrder_MessageTemplate, MakePositionText(first), MakePositionText(second)),
                new EntityReference("OrderPosition", second.OrderPositionId, second.OrderPositionName),
                new EntityReference("OrderPosition", first.OrderPositionId, first.OrderPositionName),
                new EntityReference("Order", first.OrderId, first.OrderNumber));
        }

        private static string MakePositionText(ResultExtensions.OrderPositionDto dto)
        {
            return dto.OrderPositionName != dto.PositionName
                       ? string.Format(Resources.RichChildPositionTypeTemplate, dto.PositionName)
                       : Resources.RichDefaultPositionTypeTemplate;
        }
    }
}