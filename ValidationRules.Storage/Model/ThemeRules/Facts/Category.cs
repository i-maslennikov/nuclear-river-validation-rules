namespace NuClear.ValidationRules.Storage.Model.ThemeRules.Facts
{
    public sealed class Category
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public bool IsNotActiveOrDeleted { get; set; }
    }
}