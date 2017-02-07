using System.Collections.Generic;

using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Model
{
    public sealed class ValidationResult
    {
        public MessageTypeCode Rule { get; set; }
        public string Template { get; set; }
        public IReadOnlyCollection<EntityReference> References { get; set; }
        public EntityReference MainReference { get; set; }
        public Result Result { get; set; }
    }
}