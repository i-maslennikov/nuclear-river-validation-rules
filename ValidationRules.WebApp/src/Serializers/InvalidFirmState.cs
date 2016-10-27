namespace NuClear.ValidationRules.WebApp.Serializers
{
    public enum InvalidFirmState
    {
        NotSet = 0,
        Deleted,
        ClosedForever,
        ClosedForAscertainment
    }

    public enum ReviewStatus
    {
        NotSet = 0,
        Invalid,
        Draft,
    }
}