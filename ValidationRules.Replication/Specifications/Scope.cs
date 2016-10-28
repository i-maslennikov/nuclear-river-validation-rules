using LinqToDB;

namespace NuClear.ValidationRules.Replication.Specifications
{
    public static class Scope
    {
        [Sql.Expression("case when {0} in (4, 5) then 0 when {0} = 2 then 1 when {0} = 1 then {1} else -1 end")]
        public static long Compute(int state, long id)
        {
            switch (state)
            {
                case OrderState.Approved:
                case OrderState.OnTermination:
                    return ApprovedScope;
                case OrderState.OnApproval:
                    return OnApprovalScope;
                case OrderState.OnRegistration:
                    return id;
                default:
                    return UndefinedScope;
            }
        }

        [Sql.Expression("({1} = 0 or {1} = {0} or ({1} = 1 and {0} <> 0))", IsPredicate = true)]
        public static bool CanSee(long thisScope, long otherScope)
        {
            return otherScope == ApprovedScope || otherScope == thisScope || (otherScope == OnApprovalScope && thisScope != ApprovedScope);
        }

        private static class OrderState
        {
            public const int OnRegistration = 1;
            public const int OnApproval = 2;
            public const int OnTermination = 4;
            public const int Approved = 5;
        }

        public const long ApprovedScope = 0;
        public const long OnApprovalScope = -1;
        private const long UndefinedScope = -2;
    }

}
