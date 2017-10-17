using System;

namespace NuClear.ValidationRules.Replication
{
    public struct PeriodKey
    {
        public DateTime Date { get; set; }
    }

    public struct ErmState
    {
        public ErmState(Guid token, DateTime utcDateTime)
        {
            Token = token;
            UtcDateTime = utcDateTime;
        }

        public Guid Token { get; }
        public DateTime UtcDateTime { get; }
    }

    public struct AmsState
    {
        public AmsState(long offset, DateTime utcDateTime)
        {
            Offset = offset;
            UtcDateTime = utcDateTime;
        }

        public long Offset { get; }
        public DateTime UtcDateTime { get; }
    }
}