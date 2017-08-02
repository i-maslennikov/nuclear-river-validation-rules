using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class ProjectMustContainCostPerClickMinimumRestrictionMassMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.ProjectMustContainCostPerClickMinimumRestrictionMass;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var categoryReference = references.Get<EntityTypeCategory>();
            var projectReference = references.Get<EntityTypeProject>();

            return new MessageComposerResult(
                projectReference,
                Resources.CpcRestrictionIsMissing,
                categoryReference,
                projectReference);
        }
    }
}