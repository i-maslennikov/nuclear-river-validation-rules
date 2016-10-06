using System;

namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class ReleaseWithdrawal
    {
        public long Id { get; set; }

        public long OrderPositionId { get; set; }
        public DateTime ReleaseBeginDate { get; set; }
        public decimal AmountToWithdraw { get; set; }
    }
}