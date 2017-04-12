using System;
using System.Collections.Generic;

namespace NuClear.ValidationRules.Storage.FieldComparer
{
    /// <summary>
    /// Выполняет сравнение DateTime с точностью загрубленной до 1E-2 секунды.
    /// Делаем так, потому что храним их как datetime2(2)
    /// </summary>
    public sealed class DateTimeComparer : IEqualityComparer<DateTime>
    {
        private const long Ticks = TimeSpan.TicksPerSecond / 100;

        public bool Equals(DateTime x, DateTime y)
        {
            var xRounded = x.Ticks / Ticks;
            var yRounded = y.Ticks / Ticks;
            return xRounded == yRounded;
        }

        public int GetHashCode(DateTime obj)
        {
            var rounded = obj.Ticks / Ticks;
            return rounded.GetHashCode();
        }
    }
}