namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class OrderFile
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public int FileKind { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
