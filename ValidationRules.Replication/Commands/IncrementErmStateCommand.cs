using System;
using System.Collections.Generic;

using NuClear.Replication.Core;

namespace NuClear.ValidationRules.Replication.Commands
{
    public sealed class IncrementErmStateCommand : ICommand
    {
        public IncrementErmStateCommand(IEnumerable<ErmState> states)
        {
            States = states;
        }

        public IEnumerable<ErmState> States { get; }
    }
}