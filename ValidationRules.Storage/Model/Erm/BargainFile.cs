namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class BargainFile
    {
        public const int BargainScan = 12;

        public long Id { get; set; }
        public long BargainId { get; set; }
        public int FileKind { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
