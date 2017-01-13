using System.Linq;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class SatisfiedPrincipalPositionDifferentOrderMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.SatisfiedPrincipalPositionDifferentOrder;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositions = validationResult.ReadOrderPositions();

            var master = orderPositions.First();
            var dependents = orderPositions.Skip(1).ToArray();
            var placeholders = dependents.Select((x, i) => string.Format(Resources.ADPValidation_Template_Part, MakePositionText(x), 1 + i, 1 + dependents.Length + i));

            return new MessageComposerResult(
                orderReference,
                string.Format(Resources.ADPValidation_Template, MakePositionText(master), string.Join(", ", placeholders)),
                new[] { new EntityReference("OrderPosition", master.OrderPositionId, master.OrderPositionName) }
                    .Concat(dependents.Select(x => new EntityReference("OrderPosition", x.OrderPositionId, x.OrderPositionName)))
                    .Concat(dependents.Select(x => new EntityReference("Order", x.OrderId, x.OrderNumber)))
                    .ToArray());
        }

        private static string MakePositionText(ResultExtensions.OrderPositionDto dto)
        {
            return dto.OrderPositionName != dto.PositionName
                       ? string.Format(Resources.RichChildPositionTypeTemplate, dto.PositionName)
                       : Resources.RichDefaultPositionTypeTemplate;
        }
    }
}