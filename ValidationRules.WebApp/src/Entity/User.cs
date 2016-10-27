namespace NuClear.ValidationRules.WebApp.Entity
{
    public class User
    {
        public long Id { get; set; }
        public string Account { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
