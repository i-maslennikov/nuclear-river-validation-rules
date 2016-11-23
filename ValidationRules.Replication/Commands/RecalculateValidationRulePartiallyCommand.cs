using System.Collections.Generic;

using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.Commands
{
    public class RecalculateValidationRulePartiallyCommand : RecalculateValidationRuleCommand
    {
        public RecalculateValidationRulePartiallyCommand(MessageTypeCode rule, IReadOnlyCollection<long> filter)
            : base(rule)
        {
            Filter = filter;
        }

        public IReadOnlyCollection<long> Filter { get; }
    }
}