using System;

namespace NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates
{
    public sealed class Account
    {
        public long Id { get; set; }

        public sealed class AccountPeriod
        {
            public long AccountId { get; set; }

            /// <summary>
            /// Текущее значение баланса ЛС.
            /// </summary>
            public decimal Balance { get; set; }

            /// <summary>
            /// Сумма, ожидаемая к списанию со счёта за период. Формируется по всем заказам, привязанным ко счёту.
            /// </summary>
            public decimal ReleaseAmount { get; set; }

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
}