namespace NuClear.CustomerIntelligence.Domain.Model.CI
{
    public sealed class Client
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long CategoryGroupId { get; set; }
    }
}