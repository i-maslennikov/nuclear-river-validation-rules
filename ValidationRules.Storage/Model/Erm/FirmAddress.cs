namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class FirmAddress
    {
        public long Id { get; set; }
        public long FirmId { get; set; }
        public bool ClosedForAscertainment { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string Address { get; set; }
        public bool IsLocatedOnTheMap { get; set; }
        public long? EntranceCode { get; set; }
        public int? BuildingPurposeCode { get; set; }
    }
}