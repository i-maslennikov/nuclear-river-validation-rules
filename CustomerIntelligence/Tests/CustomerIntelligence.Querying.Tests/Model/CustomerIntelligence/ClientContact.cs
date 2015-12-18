namespace NuClear.CustomerIntelligence.Querying.Tests.Model.CustomerIntelligence
{
    public sealed class ClientContact
    {
        public long ContactId { get; set; }
        public long ClientId { get; set; }
        public ContactRole Role { get; set; }
    }
}