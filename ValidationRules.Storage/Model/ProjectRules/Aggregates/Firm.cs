namespace NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates
{
    public sealed class Firm
    {
        public long Id { get; set; }

        public sealed class Address
        {
            public long Id { get; set; }
            public long FirmId { get; set; }
            public string Name { get; set; }
            public bool IsLocatedOnTheMap { get; set; }
        }
    }
}