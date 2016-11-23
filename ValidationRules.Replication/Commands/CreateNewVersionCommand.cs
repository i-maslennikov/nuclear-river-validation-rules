using System;
using System.Collections.Generic;

namespace NuClear.ValidationRules.Replication.Commands
{
    public sealed class CreateNewVersionCommand : IValidationRuleCommand
    {
        public CreateNewVersionCommand(IReadOnlyCollection<Guid> states)
        {
            States = states;
        }

        public IReadOnlyCollection<Guid> States { get; }
    }
}
