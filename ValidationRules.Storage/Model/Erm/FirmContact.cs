namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class FirmContact
    {
        public const int Website = 4;

        public long Id { get; set; }

        // nullable property https://jira.2gis.ru/browse/ERM-5252 
        public long? FirmAddressId { get; set; }

        public int ContactType { get; set; }
        public string Contact { get; set; }
    }
}