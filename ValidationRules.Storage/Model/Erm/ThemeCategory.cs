namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class ThemeCategory
    {
        public long Id { get; set; }
        public long ThemeId { get; set; }
        public long CategoryId { get; set; }
        public bool IsDeleted { get; set; }
    }
}