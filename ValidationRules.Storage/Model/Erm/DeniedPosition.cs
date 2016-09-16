namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class DeniedPosition
    {
        public long Id { get; set; }
        public long PositionId { get; set; }
        public long PositionDeniedId { get; set; }
        public long PriceId { get; set; }
        public int ObjectBindingType { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
