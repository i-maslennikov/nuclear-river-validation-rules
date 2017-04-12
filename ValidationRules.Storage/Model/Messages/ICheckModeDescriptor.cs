using System.Collections.Generic;

namespace NuClear.ValidationRules.Storage.Model.Messages
{
    public interface ICheckModeDescriptor
    {
        IReadOnlyCollection<MessageTypeCode> Rules { get; }
        RuleSeverityLevel GetRuleSeverityLevel(MessageTypeCode rule);
    }
}