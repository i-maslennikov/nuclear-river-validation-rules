using System.Collections.Generic;

using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.Commands
{
    public sealed class RecalculateValidationRulePartiallyCommand : IRecalculateValidationRuleCommand
    {
        public RecalculateValidationRulePartiallyCommand(MessageTypeCode rule, IReadOnlyCollection<long> filter)
        {
            Rule = rule;
            Filter = filter;
        }

        public MessageTypeCode Rule { get; }
        public IReadOnlyCollection<long> Filter { get; }
    }
}