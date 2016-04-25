namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public interface IKey
    {
        string Key { get; }
    }

    public sealed class Facts : IKey
    {
        public string Key => "-facts";
    }

    public sealed class Aggregates : IKey
    {
        public string Key => "-aggregates";
    }
}