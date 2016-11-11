using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Controllers
{
    internal static class ResultExtensions
    {
        private const int ShiftSingle = 0;
        private const int ShiftManual = 2;
        private const int ShiftPrerelease = 4;
        private const int ShiftRelease = 6;

        private const int ResultMask = 0x03;

        public static int SingleMask = ResultMask << ShiftSingle;
        public static int ManualMask = ResultMask << ShiftManual;
        public static int PrereleaseMask = ResultMask << ShiftPrerelease;
        public static int ReleaseMask = ResultMask << ShiftRelease;

        public static Result WhenSingle(Version.ValidationResult result)
            => GetResult(result.Result, ShiftSingle);

        public static Result WhenManual(Version.ValidationResult result)
            => GetResult(result.Result, ShiftManual);

        public static Result WhenPrerelease(Version.ValidationResult result)
            => GetResult(result.Result, ShiftPrerelease);

        public static Result WhenRelease(Version.ValidationResult result)
            => GetResult(result.Result, ShiftRelease);

        private static Result GetResult(int value, int shift)
            => (Result)(ResultMask & (value >> shift));
    }
}
