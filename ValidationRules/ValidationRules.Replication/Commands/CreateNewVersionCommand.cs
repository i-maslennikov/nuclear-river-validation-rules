using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
