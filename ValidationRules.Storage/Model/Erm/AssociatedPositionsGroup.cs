namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class AssociatedPositionsGroup
    {
        public long Id { get; set; }
        public long PricePositionId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
