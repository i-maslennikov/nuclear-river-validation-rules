using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication
{
    public sealed class ResultBuilder
    {
        private const int ShiftSingle = 0;
        private const int ShiftMass = 2;
        private const int ShiftMassPrerelease = 4;
        private const int ShiftMassRelease = 6;
        private const int ShiftMassWithAccount = 8;
        private const int ShiftSingleForTermination = 10;
        private const int ShiftSingleForApprove = 12;

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

        public ResultBuilder WhenSingle(Result result)
            => AddResult((int)result, ShiftSingle);

        public ResultBuilder WhenSingleForCancel(Result result)
            => AddResult((int)result, ShiftSingleForTermination);

        public ResultBuilder WhenSingleForApprove(Result result)
            => AddResult((int)result, ShiftSingleForApprove);

        public ResultBuilder WhenMass(Result result)
            => AddResult((int)result, ShiftMass).AddResult((int)result, ShiftMassWithAccount);

        public ResultBuilder WhenMassWithAccount(Result result)
            => AddResult((int)result, ShiftMassWithAccount);

        public ResultBuilder WhenMassPrerelease(Result result)
            => AddResult((int)result, ShiftMassPrerelease);

        public ResultBuilder WhenMassRelease(Result result)
            => AddResult((int)result, ShiftMassRelease);

        private ResultBuilder AddResult(int result, int shift)
        {
            _accumulator = _accumulator & ~(ResultMask << shift); // очистка
            _accumulator = _accumulator | result << shift; // установка
            return this;
        }
    }
}
