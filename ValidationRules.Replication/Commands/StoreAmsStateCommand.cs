namespace NuClear.ValidationRules.Replication.Commands
{
    public sealed class StoreAmsStateCommand : IValidationRuleCommand
    {
        public StoreAmsStateCommand(AmsState state)
        {
            State = state;
        }

        public AmsState State { get; }
    }
}