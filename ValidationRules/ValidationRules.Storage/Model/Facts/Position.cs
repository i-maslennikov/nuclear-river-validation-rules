namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class Position
    {
        public long Id { get; set; }
        public long CategoryCode { get; set; }
        public bool IsControlledByAmount { get; set; }
        public bool IsComposite { get; set; }
        public int CompareMode { get; set; }
        public string Name { get; set; }
    }
}
