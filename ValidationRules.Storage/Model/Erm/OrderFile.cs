namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class OrderFile
    {
        public const int OrderScan = 8;

        public long Id { get; set; }
        public long OrderId { get; set; }
        public int FileKind { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
