using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition
{
    public interface IMessageSeverityProvider
    {
        RuleSeverityLevel GetLevel(Message message, ICheckModeDescriptor checkModeDescriptor);
    }
}