using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.Commands
{
    public class RecalculateValidationRuleCommand : IValidationRuleCommand
    {
        public RecalculateValidationRuleCommand(MessageTypeCode rule)
        {
            Rule = rule;
        }

        public MessageTypeCode Rule { get; }
    }
}