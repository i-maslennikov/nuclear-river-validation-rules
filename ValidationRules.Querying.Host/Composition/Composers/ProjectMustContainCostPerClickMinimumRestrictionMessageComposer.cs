using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class ProjectMustContainCostPerClickMinimumRestrictionMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.ProjectMustContainCostPerClickMinimumRestriction;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var categoryReference = references.Get<EntityTypeCategory>();
            var projectReference = references.Get<EntityTypeProject>();

            return new MessageComposerResult(
                orderReference,
                Resources.CpcRestrictionIsMissing,
                categoryReference,
                projectReference);
        }
    }
}