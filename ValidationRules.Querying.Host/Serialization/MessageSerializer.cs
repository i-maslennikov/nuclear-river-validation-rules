using System.Collections.Generic;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.Serialization
{
    public class MessageSerializer
    {
        public IReadOnlyCollection<Model.ValidationResult> Serialize(IReadOnlyCollection<Version.ValidationResult> messages)
        {
            // banana
            return new Model.ValidationResult[0];
        }
    }
}