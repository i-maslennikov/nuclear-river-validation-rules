namespace NuClear.CustomerIntelligence.Storage.Model.Erm
{
    public sealed class Account
    {
        public long Id { get; set; }

        public decimal Balance { get; set; }

        public long BranchOfficeOrganizationUnitId { get; set; }

        public long LegalPersonId { get; set; }

        public bool IsArchived { get; set; }
    }
}