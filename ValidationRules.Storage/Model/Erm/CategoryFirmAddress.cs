namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class CategoryFirmAddress
    {
        public long Id { get; set; }
        public long FirmAddressId { get; set; }
        public long CategoryId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}