namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class AssociatedPosition
    {
        public long Id { get; set; }
        public long AssociatedPositionsGroupId { get; set; }
        public long PositionId { get; set; }
        public int ObjectBindingType { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
