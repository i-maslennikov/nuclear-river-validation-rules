namespace NuClear.ValidationRules.Storage.Model.Messages
{
    public sealed class CombinedResult
    {
        private const int ShiftSingle = 0;
        private const int ShiftManual = 2;
        private const int ShiftPrerelease = 4;
        private const int ShiftRelease = 6;

        private const int ResultMask = 0x03;

        public const int SingleMask = ResultMask << ShiftSingle;
        public const int ManualMask = ResultMask << ShiftManual;
        public const int PrereleaseMask = ResultMask << ShiftPrerelease;
        public const int ReleaseMask = ResultMask << ShiftRelease;

        public Result ForSingle { get; private set; }
        public Result ForManual { get; private set; }
        public Result ForPrerelease { get; private set; }
        public Result ForRelease { get; private set; }

        public int ToInt32()
            => (int)ForSingle << ShiftSingle |
               (int)ForManual << ShiftManual |
               (int)ForPrerelease << ShiftPrerelease |
               (int)ForRelease << ShiftRelease;

        public static CombinedResult FromInt32(int result)
            => new CombinedResult
                {
                    ForSingle = (Result)((result >> ShiftSingle) & ResultMask),
                    ForManual = (Result)((result >> ShiftManual) & ResultMask),
                    ForPrerelease = (Result)((result >> ShiftPrerelease) & ResultMask),
                    ForRelease = (Result)((result >> ShiftRelease) & ResultMask),
                };
    }
}