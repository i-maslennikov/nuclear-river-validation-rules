using System;

namespace NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates
{
    public sealed class AccountPeriod
    {
        public long AccountId { get; set; }

        /// <summary>
        /// Текущее значение баланса ЛС.
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Сумма блокировок по счёту за период.
        /// </summary>
        public decimal LockedAmount { get; set; }

        /// <summary>
        /// Сумма блокировок по счёту за всё время.
        /// </summary>
        public decimal OwerallLockedAmount { get; set; }

        /// <summary>
        /// Сумма, ожидаемая к списанию со счёта за период. Формируется по всем заказам, привязанным ко счёту.
        /// </summary>
        public decimal ReleaseAmount { get; set; }

        /// <summary>
        /// Сумма, на которую предоставлены лимиты по ЛС.
        /// </summary>
        public decimal LimitAmount { get; set; }

        /// <summary>
        /// Начало периода, за который выполнен расчёт баланса ЛС.
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Окончание периода, за каоторый выполнен расчёт баланса ЛС.
        /// </summary>
        public DateTime End { get; set; }
    }
}