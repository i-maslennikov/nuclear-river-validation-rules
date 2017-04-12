using System.Collections.Generic;

using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Model
{
    /// <summary>
    /// Тип явяляется частью контракта.
    /// </summary>
    public sealed class ValidationResult
    {
        public MessageTypeCode Rule { get; set; }
        public string Template { get; set; }
        public IReadOnlyCollection<Reference> References { get; set; }
        public Reference MainReference { get; set; }
        public RuleSeverityLevel RuleSeverityLevel { get; set; }

        public sealed class Reference
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public long Id { get; set; }
        }
    }
}