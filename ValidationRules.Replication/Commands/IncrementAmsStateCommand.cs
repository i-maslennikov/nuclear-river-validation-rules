using NuClear.Replication.Core;

namespace NuClear.ValidationRules.Replication.Commands
{
    public sealed class IncrementAmsStateCommand : ICommand
    {
        public IncrementAmsStateCommand(AmsState state)
        {
            State = state;
        }

        public AmsState State { get; }
    }
}