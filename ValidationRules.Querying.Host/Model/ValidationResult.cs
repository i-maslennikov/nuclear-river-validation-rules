using System.Collections.Generic;

namespace ValidationRules.Querying.Host.Model
{
    public class ValidationResult
    {
        public string Template { get; set; }
        public IReadOnlyCollection<EntityReference> References { get; set; }
        public EntityReference MainReference { get; set; }
        public string Result { get; set; }
    }
}