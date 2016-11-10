using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Controllers
{
    internal static class ResultExtensions
    {
        private const int ShiftSingle = 0;
        private const int ShiftMass = 2;
        private const int ShiftMassPrerelease = 4;
        private const int ShiftMassRelease = 6;

        private const int ResultMask = 0x03;

        public static Result WhenSingle(Version.ValidationResult result)
            => GetResult(result.Result, ShiftSingle);

        public static Result WhenManual(Version.ValidationResult result)
            => GetResult(result.Result, ShiftMass);

        public static Result WhenPrerelease(Version.ValidationResult result)
            => GetResult(result.Result, ShiftMassPrerelease);

        public static Result WhenRelease(Version.ValidationResult result)
            => GetResult(result.Result, ShiftMassRelease);

        private static Result GetResult(int value, int shift)
            => (Result)(ResultMask & (value >> shift));
    }
}
