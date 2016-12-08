using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.Commands
{
    public class RecalculateValidationRuleCompleteCommand : IRecalculateValidationRuleCommand
    {
        public RecalculateValidationRuleCompleteCommand(MessageTypeCode rule)
        {
            Rule = rule;
        }

        public MessageTypeCode Rule { get; }
    }
}