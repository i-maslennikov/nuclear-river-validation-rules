namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class Advertisement
    {
        public const int Ok = 0;

        public long Id { get; set; }
        public long FirmId { get; set; }
        public int StateCode { get; set; }
    }
}