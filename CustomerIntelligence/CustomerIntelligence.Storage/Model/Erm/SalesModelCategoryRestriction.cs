namespace NuClear.CustomerIntelligence.Storage.Model.Erm
{
    public sealed class SalesModelCategoryRestriction
    {
        public long Id { get; set; }
        public long CategoryId { get; set; }
        public long ProjectId { get; set; }
        public int SalesModel { get; set; }
    }
}