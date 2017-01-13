using System;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AssociatedPositionWithoutPrincipalMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AssociatedPositionWithoutPrincipal;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var position = validationResult.ReadOrderPositions().First();

            return new MessageComposerResult(
                orderReference,
                string.Format(Resources.AssociatedPositionWithoutPrincipalTemplate, MakePositionText(position)),
                new EntityReference("OrderPosition", position.OrderPositionId, position.OrderPositionName));
        }

        private static string MakePositionText(ResultExtensions.OrderPositionDto dto)
        {
            return dto.OrderPositionName != dto.PositionName
                       ? string.Format(Resources.RichChildPositionTypeTemplate, dto.PositionName)
                       : Resources.RichDefaultPositionTypeTemplate;
        }
    }
}