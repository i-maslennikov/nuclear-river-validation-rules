namespace NuClear.ValidationRules.Storage.Model.AccountRules.Facts
{
    public sealed class Account
    {
        public long Id { get; set; }
        public long BranchOfficeOrganizationUnitId { get; set; }
        public long LegalPersonId { get; set; }
        public decimal Balance { get; set; }
    }
}
