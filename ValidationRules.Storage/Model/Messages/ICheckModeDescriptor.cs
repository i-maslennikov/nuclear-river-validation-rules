using System.Collections.Generic;

namespace NuClear.ValidationRules.Storage.Model.Messages
{
    public interface ICheckModeDescriptor
    {
        IReadOnlyCollection<MessageTypeCode> Rules { get; }
        Result GetRuleSeverityLevel(MessageTypeCode rule);
    }
}