namespace NuClear.ValidationRules.Storage.Model.Messages
{
    public enum CheckMode
    {
        Single = 1,
        SingleForCancel = 2,
        SingleForApprove = 3,

        Manual = 4,
        ManualWithAccount = 5,

        Prerelease = 6,
        Release = 7,
    }
}