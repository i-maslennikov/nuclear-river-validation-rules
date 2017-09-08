using System.Collections.Generic;

namespace NuClear.ValidationRules.Replication.Commands
{
    public sealed class StoreErmStateCommand : IValidationRuleCommand
    {
        public StoreErmStateCommand(IEnumerable<ErmState> states)
        {
            States = states;
        }

        public IEnumerable<ErmState> States { get; }
    }
}
