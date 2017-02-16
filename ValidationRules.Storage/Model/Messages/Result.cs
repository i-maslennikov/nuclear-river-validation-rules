namespace NuClear.ValidationRules.Storage.Model.Messages
{
    public enum Result
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
    }
    public enum ResultType
    {
        Single = 0,
        Manual = 2,
        Prerelease = 4,
        Release = 6,
    }

    public static class ResultExtensions
    {
        private const int ResultMask = 0x03;

        public static int ToBitMask(this ResultType resultType) => ResultMask << (int)resultType;

        public static Result ToResult(this int sqlResult, ResultType resultType) => (Result)((sqlResult >> (int)resultType) & ResultMask);
    }
}