using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.Commands
{
    public interface IRecalculateValidationRuleCommand : IValidationRuleCommand
    {
        MessageTypeCode Rule { get; }
    }
}