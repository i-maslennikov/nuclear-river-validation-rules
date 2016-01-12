namespace NuClear.ValidationRules.Domain.Model.Facts
{
    public sealed class Position : IErmFactObject
    {
        public long Id { get; set; }
        public long PositionCategoryId { get; set; }
        public bool IsControlledByAmount { get; set; }
        public bool IsComposite { get; set; }
        public string Name { get; set; }
    }
}
