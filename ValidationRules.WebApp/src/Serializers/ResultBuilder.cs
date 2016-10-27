using NuClear.ValidationRules.WebApp.Model;

namespace NuClear.ValidationRules.WebApp.Serializers
{
    public sealed class ResultBuilder
    {
        private const int ShiftSingle = 0;
        private const int ShiftMass = 2;
        private const int ShiftMassPrerelease = 4;
        private const int ShiftMassRelease = 6;

        private const int ResultMask = 0x03;

        private int _accumulator;

        public ResultBuilder()
            : this(0)
        {
        }

        public ResultBuilder(int result)
        {
            _accumulator = result;
        }

        public static implicit operator int(ResultBuilder builder)
        {
            return builder._accumulator;
        }

        public Result WhenSingle()
            => GetResult(ShiftSingle);

        public Result WhenMass()
            => GetResult(ShiftMass);

        public Result WhenMassPrerelease()
            => GetResult(ShiftMassPrerelease);

        public Result WhenMassRelease()
            => GetResult(ShiftMassRelease);

        private Result GetResult(int shift)
        {
            return (Result)(ResultMask & (_accumulator >> shift));
        }
    }
}
