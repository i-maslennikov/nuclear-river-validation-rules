namespace NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates
{
    public sealed class Firm
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool NeedsSpecialPosition { get; set; }
    }
}