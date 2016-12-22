using System;
using System.Collections.Generic;

namespace NuClear.ValidationRules.SingleCheck.FieldComparer
{
    internal class DateTimeComparer : IEqualityComparer<DateTime>
    {
        public bool Equals(DateTime x, DateTime y)
        {
            return string.Equals(x.ToString("u"), y.ToString("u"));
        }

        public int GetHashCode(DateTime obj)
        {
            return obj.ToString("u").GetHashCode();
        }
    }
}