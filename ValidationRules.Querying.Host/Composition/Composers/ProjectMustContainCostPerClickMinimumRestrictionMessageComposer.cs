using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class ProjectMustContainCostPerClickMinimumRestrictionMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.ProjectMustContainCostPerClickMinimumRestriction;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var categoryReference = validationResult.ReadCategoryReference();
            var projectReference = validationResult.ReadProjectReference();

            return new MessageComposerResult(
                orderReference,
                Resources.CpcRestrictionIsMissing,
                categoryReference,
                projectReference);
        }
    }
}