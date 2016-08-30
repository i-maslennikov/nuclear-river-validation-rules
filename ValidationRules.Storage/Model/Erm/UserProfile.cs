namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class UserProfile
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int TimeZoneId { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}