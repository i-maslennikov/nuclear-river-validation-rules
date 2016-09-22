namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class User
    {
        public long Id { get; set; }
        public string Account { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsServiceUser { get; set; }
    }
}