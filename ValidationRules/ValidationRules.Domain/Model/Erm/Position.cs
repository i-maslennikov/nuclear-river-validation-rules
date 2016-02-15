namespace NuClear.ValidationRules.Domain.Model.Erm
{
    public sealed class Position
    {
        public long Id { get; set; }
        public long CategoryId { get; set; }
        public bool IsControlledByAmount { get; set; }
        public bool IsComposite { get; set; }
        public int BindingObjectTypeEnum { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
