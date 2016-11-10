using System.Collections.Generic;

using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Model
{
    public class ValidationResult
    {
        public int Rule { get; set; }
        public string Template { get; set; }
        public IReadOnlyCollection<EntityReference> References { get; set; }
        public EntityReference MainReference { get; set; }
        public Result Result { get; set; }
    }
}